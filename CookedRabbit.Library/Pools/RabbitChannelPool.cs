using CookedRabbit.Library.Models;
using CookedRabbit.Library.Utilities;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Pools
{
    /// <summary>
    /// CookedRabbit RabbitChannelPool creates the connection pool and manages the channels.
    /// </summary>
    public class RabbitChannelPool : IRabbitChannelPool
    {
        private readonly object _channeAddLock = new object();

        private ulong _currentChannelId = 0;
        private ushort _channelsToMaintain = 100;
        private ushort _emptyPoolWaitTime = 100;
        private IRabbitConnectionPool _rcp = null;
        private ConcurrentQueue<(ulong, IModel)> _channelPool = new ConcurrentQueue<(ulong, IModel)>();
        private ConcurrentBag<(ulong, IModel)> _channelPoolInUse = new ConcurrentBag<(ulong, IModel)>();
        private ConcurrentQueue<(ulong, IModel)> _channelWithManualAckPool = new ConcurrentQueue<(ulong, IModel)>();
        private ConcurrentBag<(ulong, IModel)> _channelWithManualAckPoolInUse = new ConcurrentBag<(ulong, IModel)>();
        private ConcurrentBag<ulong> _flaggedAsDeadChannels = new ConcurrentBag<ulong>();
        private RabbitSeasoning _seasoning = null; // Used if channels go null later.

        #region AutoScaling Variables

        private long _maxAutoScaleChannels = 0;
        private long _threshold = 0;

        private long _hysteresisChannelPool = 0;
        private long _scalingIncrementChannelPool = 0;

        private long _hysteresisAckChannelPool = 0;
        private long _scalingIncrementAckChannelPool = 0;

        #endregion

        /// <summary>
        /// Check to see if the channel pool has already been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        #region Constructor & Setup

        /// <summary>
        /// CookedRabbit RabbitChannelPool constructor.
        /// </summary>
        public RabbitChannelPool() { }

        /// <summary>
        /// Manually set the IRabbitConnectionPool for flexiblity in sharing the connection pool across services.
        /// <para>If RabbitConnectionPool is not initialized, it will be here.</para>
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <returns></returns>
        public async Task SetConnectionPoolAsync(RabbitSeasoning rabbitSeasoning, IRabbitConnectionPool rcp)
        {
            if (_rcp != null) throw new SystemException("Can't assign a connection pool, one is already assigned.");

            _rcp = rcp;

            if (!_rcp.IsInitialized)
            { await _rcp.Initialize(rabbitSeasoning); }

            if (!IsInitialized)
            { await Initialize(rabbitSeasoning); }
        }

        /// <summary>
        /// RabbitChannelPool initialization method.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <returns></returns>
        public async Task Initialize(RabbitSeasoning rabbitSeasoning)
        {
            if (rabbitSeasoning is null) throw new ArgumentNullException(nameof(rabbitSeasoning));

            _shutdown = false;
            _seasoning = rabbitSeasoning;

            if (_rcp is null) _rcp = await Factories.CreateRabbitConnectionPoolAsync(_seasoning);

            if (!IsInitialized)
            {
                _maxAutoScaleChannels = rabbitSeasoning.PoolSettings.MaxAutoScaleChannels;
                _threshold = rabbitSeasoning.PoolSettings.ScalingWaitThreshold;
                _channelsToMaintain = rabbitSeasoning.PoolSettings.ChannelPoolCount;
                _emptyPoolWaitTime = rabbitSeasoning.PoolSettings.EmptyPoolWaitTime;

                await CreatePoolChannels();
                await CreatePoolChannelsWithManualAck();

                IsInitialized = true;
            }
        }

        private Task CreatePoolChannels()
        {
            lock (_channeAddLock)
            {
                for (int i = 0; i < _channelsToMaintain; i++)
                {
                    _channelPool.Enqueue((_currentChannelId++, _rcp.GetConnection().CreateModel()));
                }
            }

            return Task.CompletedTask;
        }

        private Task CreatePoolChannelsWithManualAck()
        {
            lock (_channeAddLock)
            {
                for (int i = 0; i < _channelsToMaintain; i++)
                {
                    var channel = _rcp.GetConnection().CreateModel();
                    channel.ConfirmSelect();

                    _channelWithManualAckPool.Enqueue((_currentChannelId++, channel));
                }
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Channels & Maintenance Section

        /// <summary>
        /// Gets the current channel count by reading the current ChannelId.
        /// </summary>
        /// <returns></returns>
        public long GetCurrentChannelCount()
        {
            var channelId = (long)_currentChannelId;
            return Interlocked.Read(ref channelId);
        }

        /// <summary>
        /// Gets the number of times AutoScaling was triggered in the channel pool.
        /// </summary>
        /// <returns></returns>
        public long GetChannelPoolAutoScalingIterationCount()
        {
            return Interlocked.Read(ref _scalingIncrementChannelPool);
        }

        /// <summary>
        /// Gets the number of times AutoScaling was triggered in the ackable channel pool.
        /// </summary>
        /// <returns></returns>
        public long GetAckableChannelPoolAutoScalingIterationCount()
        {
            return Interlocked.Read(ref _scalingIncrementAckChannelPool);
        }

        /// <summary>
        /// Creates a transient (untracked) RabbitMQ channel. Closing/Disposal is the responsibility of the calling service.
        /// </summary>
        /// <param name="enableAck"></param>
        /// <returns>Returns an IModel channel (RabbitMQ).</returns>
        public async Task<IModel> GetTransientChannelAsync(bool enableAck = false)
        {
            var t = await Task.Run(() => // Helps decouple Tasks from any calling thread.
            {
                IModel channel = null;

                try
                {
                    channel = _rcp.GetConnection().CreateModel();

                    if (enableAck)
                    { channel.ConfirmSelect(); }
                }
                catch { } // TODO

                return channel;
            });

            return t;
        }

        /// <summary>
        /// Gets a pre-created (tracked) RabbitMQ channel. Must be returned by the calling the service!
        /// </summary>
        /// <returns>Returns a ValueTuple(ulong, IModel)</returns>
        public async Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPairAsync()
        {
            var t = await Task.Run(async () => // Helps decouple Tasks from any calling thread.
            {
                var keepLoopingUntilChannelAcquired = true;
                (ulong ChannelId, IModel Channel) channelPair = (0UL, null);

                while (keepLoopingUntilChannelAcquired)
                {
                    if (_channelPool.TryDequeue(out channelPair))
                    {
                        if (_flaggedAsDeadChannels.Contains(channelPair.ChannelId))
                        {
                            // Replace channel with a new Channel, same Id.
                            channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                            _channelPoolInUse.Add(channelPair);

                            _flaggedAsDeadChannels.TryTake(out ulong noLongerDeadChannel);
                        }
                        else if (channelPair.Channel == null)
                        {
                            // Replace channel with a new Channel, same Id.
                            channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                            _channelPoolInUse.Add(channelPair);
                        }
                        else
                        { _channelPoolInUse.Add(channelPair); }

                        keepLoopingUntilChannelAcquired = false;
                    }
                    else
                    {
                        if (_seasoning.PoolSettings.EnableAutoScaling)
                        {
                            var currentScaledChannelCount = Interlocked.Read(ref _scalingIncrementChannelPool);
                            var maxScaledChannelCount = Interlocked.Read(ref _maxAutoScaleChannels);

                            // MaxAutoScale & Short Circuit
                            if (currentScaledChannelCount < maxScaledChannelCount)
                            {
                                // Read current hysteresis compare to threshold.
                                var localHysteresis = Interlocked.Read(ref _hysteresisChannelPool);
                                var localThreshold = Interlocked.Read(ref _threshold);

                                // Time to add a new channel?
                                if (localHysteresis >= localThreshold)
                                {
                                    lock (_channeAddLock)
                                    {
                                        try
                                        {
                                            _channelPool.Enqueue((_currentChannelId++, _rcp.GetConnection().CreateModel()));
                                            Interlocked.Increment(ref _scalingIncrementChannelPool);

                                            // Reset hysteresis back to zero.
                                            Interlocked.Add(ref _hysteresisChannelPool, _hysteresisChannelPool * -1);
                                        }
                                        catch { }
                                    }
                                }
                                else // No.
                                { Interlocked.Increment(ref _hysteresisChannelPool); }
                            }

                            await Task.Delay(_emptyPoolWaitTime); // Always wait.
                        }
                        else
                        {
                            if (_seasoning.PoolSettings.WriteSleepNoticeToConsole)
                            { await Console.Out.WriteLineAsync($"No channels available sleeping for {_emptyPoolWaitTime}ms."); }

                            await Task.Delay(_emptyPoolWaitTime);
                        }
                    }
                }

                return channelPair;
            });

            return t;
        }

        /// <summary>
        /// Gets a pre-created (tracked) RabbitMQ channel that can acknowledge messages. Must be returned by the calling the service!
        /// </summary>
        /// <returns>Returns a ValueTuple(ulong, IModel)</returns>
        public async Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPairAckableAsync()
        {
            var t = await Task.Run(async () => // Helps decouple Tasks from any calling thread.
            {
                var keepLoopingUntilChannelAcquired = true;
                (ulong ChannelId, IModel Channel) channelPair = (0UL, null);

                while (keepLoopingUntilChannelAcquired)
                {
                    if (_channelPool.TryDequeue(out channelPair))
                    {
                        if (_flaggedAsDeadChannels.Contains(channelPair.ChannelId))
                        {
                            // Create a new Channel
                            channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                            channelPair.Channel.ConfirmSelect();
                            _channelWithManualAckPoolInUse.Add(channelPair);

                            _flaggedAsDeadChannels.TryTake(out ulong noLongerDeadChannel);
                        }
                        else if (channelPair.Channel == null)
                        {
                            // Create a new Channel
                            channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                            channelPair.Channel.ConfirmSelect();
                            _channelWithManualAckPoolInUse.Add(channelPair);
                        }
                        else
                        {
                            _channelWithManualAckPoolInUse.Add(channelPair);
                        }

                        keepLoopingUntilChannelAcquired = false;
                    }
                    else
                    {
                        if (_seasoning.PoolSettings.EnableAutoScaling)
                        {
                            var currentScaledChannelCount = Interlocked.Read(ref _scalingIncrementAckChannelPool);
                            var maxScaledChannelCount = Interlocked.Read(ref _maxAutoScaleChannels);

                            // MaxAutoScale & Short Circuit
                            if (currentScaledChannelCount < maxScaledChannelCount)
                            {
                                // Read current hysteresis compare to threshold.
                                var localHysteresis = Interlocked.Read(ref _hysteresisAckChannelPool);
                                var localThreshold = Interlocked.Read(ref _threshold);

                                // Time to add a new channel?
                                if (localHysteresis >= localThreshold)
                                {
                                    lock (_channeAddLock)
                                    {
                                        var channelId = (long)_currentChannelId;

                                        _channelWithManualAckPool.Enqueue((_currentChannelId++, _rcp.GetConnection().CreateModel()));
                                        Interlocked.Increment(ref _scalingIncrementAckChannelPool);

                                        // Reset hysteresis back to zero.
                                        Interlocked.Add(ref _hysteresisAckChannelPool, _hysteresisAckChannelPool * -1);
                                    }
                                }
                                else // No.
                                { Interlocked.Increment(ref _hysteresisAckChannelPool); }
                            }

                            await Task.Delay(_emptyPoolWaitTime);
                        }
                        else
                        {
                            if (_seasoning.PoolSettings.WriteSleepNoticeToConsole)
                            { await Console.Out.WriteLineAsync($"No ackable channels available sleeping for {_emptyPoolWaitTime}ms."); }

                            await Task.Delay(_emptyPoolWaitTime);
                        }
                    }
                }

                return channelPair;
            });

            return t;
        }

        /// <summary>
        /// Adds ChannelId to a ConcurrentBag. This indicates it will be removed on it's next turn for usage and a new channel will be created instead of using this one.
        /// </summary>
        /// <param name="deadChannelId"></param>
        public void FlagDeadChannel(ulong deadChannelId)
        {
            if (!_flaggedAsDeadChannels.Contains(deadChannelId))
            { _flaggedAsDeadChannels.Add(deadChannelId); }
        }

        /// <summary>
        /// Called to return a channel (ackable) to its channel pool.
        /// </summary>
        /// <param name="ChannelPair"></param>
        /// <returns>Returns a bool indicating success or failure.</returns>
        public bool ReturnChannelToPool((ulong ChannelId, IModel Channel) ChannelPair)
        {
            var success = false;

            if (_channelPoolInUse.Contains(ChannelPair))
            {
                _channelPool.Enqueue(ChannelPair);
                if (!_channelPoolInUse.TryTake(out (ulong ChannelId, IModel Channel) removedChannelPair))
                {
                    Console.WriteLine($"Unable to remove channel from in-use pool {ChannelPair.ChannelId}");
                }
            }

            return success;
        }

        /// <summary>
        /// Called to return a channel (ackable) to its channel pool.
        /// </summary>
        /// <param name="ChannelPair"></param>
        /// <returns>Returns a bool indicating success or failure.</returns>
        public bool ReturnChannelToAckPool((ulong ChannelId, IModel Channel) ChannelPair)
        {
            var success = false;

            if (_channelWithManualAckPoolInUse.Contains(ChannelPair))
            {
                _channelWithManualAckPool.Enqueue(ChannelPair);
                if (!_channelWithManualAckPoolInUse.TryTake(out (ulong ChannelId, IModel Channel) removedChannelPair))
                {
                    Console.WriteLine($"Unable to remove channel from in-use pool {ChannelPair.ChannelId}");
                }
            }

            return success;
        }

        public void CloseConnections()
        {
            _rcp.CloseConnections();
        }

        #endregion

        #region Shutdown Section

        private readonly int _timeout = 10;
        private readonly object _shutdownLockObj = new object();
        private bool _shutdown = false;

        /// <summary>
        /// RabbitChannelPool shutdown method closes all channels, disposes each model, and refreshes the objects in memory.
        /// </summary>
        public void Shutdown()
        {
            if (Monitor.TryEnter(_shutdownLockObj, TimeSpan.FromSeconds(_timeout)))
            {
                if (!_shutdown)
                {
                    try
                    {
                        _shutdown = true;

                        foreach (var channelPair in _channelPool)
                        {
                            try
                            {
                                channelPair.Item2.Close(200, $"CookedRabbit channel (ChannelId:{channelPair.Item1}) shutting down.");
                                channelPair.Item2.Dispose();
                            }
                            catch { }
                        }

                        _channelPool = new ConcurrentQueue<(ulong, IModel)>();
                        _channelPoolInUse = new ConcurrentBag<(ulong, IModel)>();

                        foreach (var channelPair in _channelWithManualAckPool)
                        {
                            try
                            {
                                channelPair.Item2.Close(200, $"CookedRabbit ackable channel (ChannelId:{channelPair.Item1}) shutting down.");
                                channelPair.Item2.Dispose();
                            }
                            catch { }
                        }

                        _channelWithManualAckPool = new ConcurrentQueue<(ulong, IModel)>();
                        _channelWithManualAckPoolInUse = new ConcurrentBag<(ulong, IModel)>();

                        _rcp.Shutdown();
                    }
                    finally { Monitor.Exit(_shutdownLockObj); }
                }
            }
        }

        #endregion
    }
}
