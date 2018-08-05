using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Pools
{
    /// <summary>
    /// CookedRabbit RabbitConnectionPool creates the connections and manages the connection usage.
    /// </summary>
    public class RabbitConnectionPool : IRabbitConnectionPool
    {
        private ushort _connectionsToMaintain = 1;
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
            _shutdown = false;
            _seasoning = rabbitSeasoning;

            if (_connectionFactory is null)
            {
                _connectionFactory = await CreateConnectionFactoryAsync().ConfigureAwait(false);
                if (_connectionFactory is null) throw new ArgumentNullException("Connection factory is null.");
            }

            if (!IsInitialized)
            {
                _connectionsToMaintain = _seasoning.PoolSettings.ConnectionPoolCount;

                await CreateConnectionsAsync(_seasoning.PoolSettings.ConnectionName).ConfigureAwait(false);

                IsInitialized = true;
            }
        }

        private Task<ConnectionFactory> CreateConnectionFactoryAsync()
        {
            ConnectionFactory cf = null;

            try
            {
                if (_seasoning.FactorySettings.UseUri)
                {
                    cf = new ConnectionFactory
                    {
                        Uri = _seasoning.FactorySettings.Uri
                    };
                }
                else
                {
                    cf = new ConnectionFactory
                    {
                        UserName = _seasoning.FactorySettings.RabbitHostUser,
                        Password = _seasoning.FactorySettings.RabbitHostPassword,
                        HostName = _seasoning.FactorySettings.RabbitHostName,
                        VirtualHost = _seasoning.FactorySettings.RabbitVHost,
                        Port = _seasoning.FactorySettings.RabbitPort
                    };
                }

                cf.AutomaticRecoveryEnabled = _seasoning.FactorySettings.AutoRecovery;
                cf.TopologyRecoveryEnabled = _seasoning.FactorySettings.TopologyRecovery;
                cf.NetworkRecoveryInterval = TimeSpan.FromSeconds(_seasoning.FactorySettings.NetRecoveryTimeout);
                cf.ContinuationTimeout = TimeSpan.FromSeconds(_seasoning.FactorySettings.ContinuationTimeout);
                cf.RequestedHeartbeat = _seasoning.FactorySettings.HeartbeatInterval;
                cf.RequestedChannelMax = _seasoning.FactorySettings.MaxChannelsPerConnection;
                cf.DispatchConsumersAsync = _seasoning.FactorySettings.EnableDispatchConsumersAsync;
                cf.UseBackgroundThreadsForIO = _seasoning.FactorySettings.UseBackgroundThreadsForIO;

                if (_seasoning.SslSettings.EnableSsl)
                {
                    cf.Ssl = new SslOption
                    {
                        Enabled = true,
                        AcceptablePolicyErrors = _seasoning.SslSettings.AcceptedPolicyErrors,
                        ServerName = _seasoning.SslSettings.CertServerName,
                        CertPath = _seasoning.SslSettings.LocalCertPath,
                        CertPassphrase = _seasoning.SslSettings.LocalCertPassword,
                        Version = _seasoning.SslSettings.ProtocolVersions
                    };
                }
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
                    if (_seasoning.WriteErrorsToConsole) { await Console.Out.WriteLineAsync(ane.Message); }
                    throw; // Non Optional Throw
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException bue)
                {
                    if (_seasoning.WriteErrorsToConsole) { await Console.Out.WriteLineAsync(bue.Message); }
                    throw; // Non Optional Throw
                }
            }
        }

        private Task<IConnection> CreateConnection(string connectionName = null)
        {
            IConnection connection = null;

            try
            { connection = _connectionFactory.CreateConnection(connectionName); }
            catch
            { throw; } // TODO: Add work here.

            return Task.FromResult(connection);
        }

        #endregion

        /// <summary>
        /// Public method to get a connection manually. Lifetime is the responsiblity of the calling service.
        /// </summary>
        /// <returns>Returns an IConnection (RabbitMQ).</returns>
        public IConnection GetConnection()
        {
            if (_shutdown) throw new Exception("Connection pool is shut down. Class needs to Initialize again to use object.");

            if (_connectionPool.TryDequeue(out IConnection connection))
            {
                if (connection != null)
                { _connectionPool.Enqueue(connection); }
                else
                { connection = _connectionFactory.CreateConnection(_seasoning.PoolSettings.ConnectionName); }
            }

            return connection;
        }

        /// <summary>
        /// Allows for connections to be manually closed.
        /// </summary>
        public void CloseConnections()
        {
            foreach (var connection in _connectionPool)
            {
                try
                { connection.Close(200, "Manual close initiated."); }
                catch { }
            }
        }

        #region Shutdown Section

        private readonly int _timeout = 10;
        private readonly object _lockObj = new object();
        private bool _shutdown = false;

        /// <summary>
        /// RabbitConnectionPool shutdown method closes all connections and disposes them.
        /// </summary>
        public void Shutdown()
        {
            if (Monitor.TryEnter(_lockObj, TimeSpan.FromSeconds(_timeout)))
            {
                try
                {
                    if (!_shutdown)
                    {
                        _shutdown = true;

                        foreach (var connection in _connectionPool)
                        {
                            try
                            {
                                connection.Close(200, $"CookedRabbit connection ({connection.ClientProvidedName}) shutting down.");
                                connection.Dispose();
                            }
                            catch { }
                        }

                        _connectionPool = new ConcurrentQueue<IConnection>();
                        _connectionFactory = null;
                        _seasoning = null;
                    }
                }
                finally { Monitor.Exit(_lockObj); }
            }
        }

        #endregion
    }
}
