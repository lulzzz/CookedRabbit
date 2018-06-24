using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Pools
{
    public class RabbitChannelPool : IDisposable
    {
        private ulong _channelId = 0;
        private const short _channelsToMaintain = 100;
        private RabbitConnectionPool _rcp = null;
        private ConcurrentQueue<(ulong, IModel)> _channelPool = new ConcurrentQueue<(ulong, IModel)>();
        private ConcurrentBag<(ulong, IModel)> _channelPoolInUse = new ConcurrentBag<(ulong, IModel)>();
        private ConcurrentQueue<(ulong, IModel)> _channelWithManualAckPool = new ConcurrentQueue<(ulong, IModel)>();
        private ConcurrentBag<(ulong, IModel)> _channelWithManualAckPoolInUse = new ConcurrentBag<(ulong, IModel)>();
        private ConcurrentBag<ulong> _flaggedAsDeadChannels = new ConcurrentBag<ulong>();

        #region Constructor & Setup

        private RabbitChannelPool() { }

        public static async Task<RabbitChannelPool> CreateRabbitChannelPoolAsync(string rabbitHostName, string localHostName)
        {
            RabbitChannelPool rcp = new RabbitChannelPool();
            await rcp.Initialize(rabbitHostName, localHostName);
            return rcp;
        }

        private async Task Initialize(string rabbitHostName, string localHostName)
        {
            if (_rcp is null)
            {
                _rcp = await RabbitConnectionPool.CreateRabbitConnectionPoolAsync(rabbitHostName, localHostName);

                await CreatePoolChannels();
                await CreatePoolChannelsWithManualAck();
            }
        }

        private Task CreatePoolChannels()
        {
            for (int i = 0; i < _channelsToMaintain; i++)
            {
                _channelPool.Enqueue((_channelId++, _rcp.GetConnection().CreateModel()));
            }

            return Task.CompletedTask;
        }

        private Task CreatePoolChannelsWithManualAck()
        {
            for (int i = 0; i < _channelsToMaintain; i++)
            {
                var channel = _rcp.GetConnection().CreateModel();
                channel.ConfirmSelect();

                _channelWithManualAckPool.Enqueue((_channelId++, channel));
            }

            return Task.CompletedTask;
        }

        #endregion

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
                            // Create a new Channel
                            channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                            _channelPoolInUse.Add(channelPair);

                            _flaggedAsDeadChannels.TryTake(out ulong noLongerDeadChannel);
                        }
                        else if (channelPair.Channel == null)
                        {
                            // Create a new Channel
                            channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                            _channelPoolInUse.Add(channelPair);
                        }
                        else
                        { _channelPoolInUse.Add(channelPair); }

                        keepLoopingUntilChannelAcquired = false;
                    }
                    else
                    { await Task.Delay(100); }
                }

                return channelPair;
            });

            return t;
        }

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
                    { await Task.Delay(100); }
                }

                return channelPair;
            });

            return t;
        }

        public void FlagDeadChannel(ulong deadChannelId)
        {
            if (!_flaggedAsDeadChannels.Contains(deadChannelId))
            { _flaggedAsDeadChannels.Add(deadChannelId); }
        }

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

        #region Dispose

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { _rcp.Dispose(true); }

                _disposedValue = true;
            }
        }

        void IDisposable.Dispose() { Dispose(true); }

        #endregion
    }
}
