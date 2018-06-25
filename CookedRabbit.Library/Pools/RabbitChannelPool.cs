using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Pools
{
    public class RabbitChannelPool : IDisposable
    {
        private ulong _channelId = 0;
        private ushort _channelsToMaintain = 100;
        private ushort _emptyPoolWaitTime = 100;
        private RabbitConnectionPool _rcp = null;
        private ConcurrentQueue<(ulong, IModel)> _channelPool = new ConcurrentQueue<(ulong, IModel)>();
        private ConcurrentBag<(ulong, IModel)> _channelPoolInUse = new ConcurrentBag<(ulong, IModel)>();
        private ConcurrentQueue<(ulong, IModel)> _channelWithManualAckPool = new ConcurrentQueue<(ulong, IModel)>();
        private ConcurrentBag<(ulong, IModel)> _channelWithManualAckPoolInUse = new ConcurrentBag<(ulong, IModel)>();
        private ConcurrentBag<ulong> _flaggedAsDeadChannels = new ConcurrentBag<ulong>();
        private RabbitSeasoning _originalRabbitSeasoning = null; // Used if channels go null later.

        #region Constructor & Setup

        private RabbitChannelPool() { }

        public static async Task<RabbitChannelPool> CreateRabbitChannelPoolAsync(RabbitSeasoning rabbitSeasoning)
        {
            RabbitChannelPool rcp = new RabbitChannelPool();
            await rcp.Initialize(rabbitSeasoning);
            return rcp;
        }

        private async Task Initialize(RabbitSeasoning rabbitSeasoning)
        {
            if (_rcp is null)
            {
                _originalRabbitSeasoning = rabbitSeasoning;
                _channelsToMaintain = rabbitSeasoning.ChannelPoolCount;
                _emptyPoolWaitTime = rabbitSeasoning.EmptyPoolWaitTime;

                _rcp = await RabbitConnectionPool.CreateRabbitConnectionPoolAsync(rabbitSeasoning);

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

        #region Channels & Maintenance Section

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
                    {
                        await Console.Out.WriteLineAsync($"No channels available sleeping for {_emptyPoolWaitTime}ms.");
                        await Task.Delay(_emptyPoolWaitTime);
                    }
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

        #endregion

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
