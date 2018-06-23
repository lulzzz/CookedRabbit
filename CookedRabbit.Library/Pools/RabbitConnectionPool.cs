using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Pools
{
    public class RabbitConnectionPool : IDisposable
    {
        private const short _connectionsToMaintain = 10;
        private ConnectionFactory _connectionFactory = null;
        private ConcurrentQueue<IConnection> _connectionPool = new ConcurrentQueue<IConnection>();

        #region Constructor & Setup

        private RabbitConnectionPool()
        { }

        public static async Task<RabbitConnectionPool> CreateRabbitConnectionPoolAsync(string rabbitHostName, string connectionName)
        {
            RabbitConnectionPool rcp = new RabbitConnectionPool();
            await rcp.Initialize(rabbitHostName, connectionName);
            return rcp;
        }

        private async Task Initialize(string rabbitHostName, string connectionName)
        {
            if (_connectionFactory is null)
            {
                _connectionFactory = await CreateConnectionFactoryAsync(rabbitHostName);

                await CreateConnectionsAsync(connectionName);
            }
        }

        private Task<ConnectionFactory> CreateConnectionFactoryAsync(string rabbitHostName)
        {
            ConnectionFactory cf = null;

            try
            {
                cf = new ConnectionFactory
                {
                    HostName = rabbitHostName,
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedHeartbeat = 15,
                    RequestedChannelMax = 1000
                };
            }
            catch { cf = null; }

            return Task.FromResult(cf);
        }

        private async Task CreateConnectionsAsync(string connectionName)
        {
            for (int i = 0; i < _connectionsToMaintain; i++)
            {
                try
                { _connectionPool.Enqueue(await CreateConnection($"{connectionName}:PoolConnection:{i}")); }
                catch(RabbitMQ.Client.Exceptions.BrokerUnreachableException bue)
                {
                    await Console.Out.WriteLineAsync(bue.Message);
                }
            }
        }

        private Task<IConnection> CreateConnection(string connectionName = null)
        {
            IConnection connection = null;

            try
            { connection = _connectionFactory.CreateConnection(connectionName); }
            catch { } // TODO

            return Task.FromResult(connection);
        }

        #endregion

        public IConnection GetConnection()
        {
            if (_connectionPool.TryDequeue(out IConnection connection))
            {
                if (connection != null) { _connectionPool.Enqueue(connection); }
            }

            return connection;
        }

        #region Dispose

        private bool _disposedValue = false;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var connection in _connectionPool)
                    {
                        try
                        { connection?.Close(200, "Happily shutting down."); }
                        catch { }

                        connection?.Dispose();
                    }

                    _connectionFactory = null;
                    _connectionPool = null;
                }

                _disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
