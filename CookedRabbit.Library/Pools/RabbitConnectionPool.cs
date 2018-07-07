using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Pools
{
    /// <summary>
    /// CookedRabbit RabbitConnectionPool creates the connections and manages the connection usage.
    /// </summary>
    public class RabbitConnectionPool : IRabbitConnectionPool
    {
        private ushort _connectionsToMaintain = 10;
        private ConnectionFactory _connectionFactory = null;
        private ConcurrentQueue<IConnection> _connectionPool = new ConcurrentQueue<IConnection>();
        private RabbitSeasoning _seasoning = null; // Used if connections go null later.

        /// <summary>
        /// Check to see if the channel pool has already been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        #region Constructor & Setup

        /// <summary>
        /// CookedRabbit RabbitConnectionPool constructor.
        /// </summary>
        public RabbitConnectionPool()
        { }

        /// <summary>
        /// Initializes the RabbitConnectionPool for use.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <returns></returns>
        public async Task Initialize(RabbitSeasoning rabbitSeasoning)
        {
            if (_connectionFactory is null)
            {
                _seasoning = rabbitSeasoning;
                _connectionsToMaintain = rabbitSeasoning.ConnectionPoolCount;

                _connectionFactory = await CreateConnectionFactoryAsync(rabbitSeasoning);
                if (_connectionFactory is null) throw new ArgumentNullException("Connection factory is null.");

                await CreateConnectionsAsync(rabbitSeasoning.ConnectionName);

                IsInitialized = true;
            }
        }

        private Task<ConnectionFactory> CreateConnectionFactoryAsync(RabbitSeasoning rabbitSeasoning)
        {
            ConnectionFactory cf = null;

            try
            {
                cf = new ConnectionFactory
                {
                    HostName = rabbitSeasoning.RabbitHost,
                    AutomaticRecoveryEnabled = rabbitSeasoning.AutoRecovery,
                    TopologyRecoveryEnabled = rabbitSeasoning.TopologyRecovery,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(rabbitSeasoning.NetRecoveryTimeout),
                    RequestedHeartbeat = rabbitSeasoning.HeartbeatInterval,
                    RequestedChannelMax = rabbitSeasoning.MaxChannelsPerConnection,
                    DispatchConsumersAsync = rabbitSeasoning.EnableDispatchConsumersAsync
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

        /// <summary>
        /// Public method to get a connection manually. Lifetime is the responsiblity of the calling service.
        /// </summary>
        /// <returns>Returns an IConnection (RabbitMQ).</returns>
        public IConnection GetConnection()
        {
            if (_connectionPool.TryDequeue(out IConnection connection))
            {
                if (connection != null)
                { _connectionPool.Enqueue(connection); }
                else
                { connection = _connectionFactory.CreateConnection(_seasoning?.ConnectionName); }
            }

            return connection;
        }

        #region Shutdown Section

        private bool _shutdown = false;

        /// <summary>
        /// RabbitConnectionPool shutdown method closes all connections and disposes them.
        /// </summary>
        public void Shutdown()
        {
            if (!_shutdown)
            {
                _shutdown = true;
                foreach (var connection in _connectionPool)
                {
                    try
                    {
                        connection.Close(200, "CookedRabbit shutting down.");
                        connection.Dispose();
                    }
                    catch { }
                }

                _connectionPool = new ConcurrentQueue<IConnection>();
                _connectionFactory = null;
            }
        }

        #endregion
    }
}
