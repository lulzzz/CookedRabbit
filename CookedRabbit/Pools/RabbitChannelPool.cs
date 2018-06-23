using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CookedRabbit.Pools
{
    public class RabbitChannelPool : IDisposable
    {
        private ulong _channelId = 0;
        private const short _channelsToMaintain = 100;
        private RabbitConnectionPool _rcp = null;
        private ConcurrentQueue<(ulong, IModel)> _channelPool = new ConcurrentQueue<(ulong, IModel)>();
        private ConcurrentQueue<(ulong, IModel)> _channelWithManualAckPool = new ConcurrentQueue<(ulong, IModel)>();
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

        public async Task<IModel> GetTransientChannelAsync()
        {
            var t = await Task.Run(() => // Helps decouples from any calling thread.
            {
                IModel channel = null;

                try
                { channel = _rcp.GetConnection().CreateModel(); }
                catch { } // TODO

                return channel;
            });

            return t;
        }

        public async Task<IModel> GetTransientChannelWithManualAckAsync()
        {
            var t = await Task.Run(() => // Helps decouples from any calling thread.
            {
                IModel channel = null;

                try
                {
                    channel = _rcp.GetConnection().CreateModel();
                    channel.ConfirmSelect();
                }
                catch { } // TODO

                return channel;
            });

            return t;
        }

        // TODO: Upon pulling a channel, move to channel to InUse Pool, calling service must move it back.
        // This prevents any issue of the same channel being used at the same time in concurrency.
        // TODO: A very simple await until channels available if out of channels.
        public async Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPairAsync()
        {
            var t = await Task.Run(() => // Helps decouples from any calling thread.
            {
                if (_channelPool.TryDequeue(out (ulong ChannelId, IModel Channel) channelPair))
                {
                    if (_flaggedAsDeadChannels.Contains(channelPair.ChannelId))
                    {
                        channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                        _channelPool.Enqueue(channelPair);

                        _flaggedAsDeadChannels.TryTake(out ulong noLongerDeadChannel);
                    }
                    else if (channelPair.Channel == null)
                    {
                        channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                        _channelPool.Enqueue(channelPair);
                    }
                    else
                    { _channelPool.Enqueue(channelPair); }
                }

                return channelPair;
            });

            return t;
        }

        public async Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPairWithManualAckAsync()
        {
            var t = await Task.Run(() => // Helps decouples from any calling thread.
            {
                if (_channelWithManualAckPool.TryDequeue(out (ulong ChannelId, IModel Channel) channelPair))
                {
                    if (_flaggedAsDeadChannels.Contains(channelPair.ChannelId))
                    {
                        channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                        channelPair.Channel.ConfirmSelect();

                        _channelWithManualAckPool.Enqueue(channelPair);

                        _flaggedAsDeadChannels.TryTake(out ulong noLongerDeadChannel);
                    }
                    else if (channelPair.Channel == null)
                    {
                        channelPair = (channelPair.ChannelId, _rcp.GetConnection().CreateModel());
                        channelPair.Channel.ConfirmSelect();

                        _channelWithManualAckPool.Enqueue(channelPair);
                    }
                    else
                    { _channelWithManualAckPool.Enqueue(channelPair); }
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
