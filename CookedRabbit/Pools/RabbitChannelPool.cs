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
        public ConcurrentQueue<(ulong, IModel)> _channelPool = new ConcurrentQueue<(ulong, IModel)>();
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
            }
        }

        private Task CreatePoolChannels()
        {
            for(int i = 0; i < _channelsToMaintain; i++)
            {
                _channelPool.Enqueue((_channelId++, _rcp.GetConnection().CreateModel()));
            }

            return Task.CompletedTask;
        }

        #endregion

        public Task<IModel> GetTransientChannel()
        {
            IModel channel = null;

            try
            {
                channel = _rcp.GetConnection().CreateModel();
            }
            catch { } // TODO

            return Task.FromResult(channel);
        }

        public async Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPair()
        {
            var t = await Task.Run(() =>
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

        public void FlagDeadChannel(ulong deadChannelId)
        {
            _flaggedAsDeadChannels.Add(deadChannelId);
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
