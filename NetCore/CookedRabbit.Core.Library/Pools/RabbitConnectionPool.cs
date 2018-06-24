using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Pools
{
    public class RabbitConnectionPool : IDisposable
    {
        private const short _connectionsToMaintain = 10;
        private ConnectionFactory _connectionFactory = null;
        private ConcurrentQueue<IConnection> _connectionPool = new ConcurrentQueue<IConnection>();
        private string ConnectionNamePrefix = string.Empty; // Used if connections go null later.

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
                if (_connectionFactory is null) throw new ArgumentNullException("Connection factory is null.");

                ConnectionNamePrefix = connectionName;
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
                    RequestedChannelMax = 1000,
                    DispatchConsumersAsync = true
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
                {
                    var connection = await CreateConnection($"{connectionName}:PoolConnection:{i}");
                    if (connection is null) throw new ArgumentNullException(nameof(connection));

                    _connectionPool.Enqueue(connection);
                }
                catch (ArgumentNullException ane)
                {
                    await Console.Out.WriteLineAsync(ane.Message);
                    throw;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException bue)
                {
                    await Console.Out.WriteLineAsync(bue.Message);
                    throw;
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
                if (connection != null)
                { _connectionPool.Enqueue(connection); }
                else
                { connection = _connectionFactory.CreateConnection(ConnectionNamePrefix); }
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
