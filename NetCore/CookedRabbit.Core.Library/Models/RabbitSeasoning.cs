using System;
using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// Class to fully season RabbitServices to your taste!
    /// </summary>
    public class RabbitSeasoning
    {
        #region CookedRabbit PerformanceService Configurable Settings

        /// <summary>
        /// Enables the use of compression and decompression.
        /// </summary>
        public bool CompressionEnabled { get; set; } = true;

        /// <summary>
        /// Sets the compression method when compression is used.
        /// </summary>
        public CompressionMethod CompressionMethod { get; set; } = CompressionMethod.LZ4;

        /// <summary>
        /// Sets the serialization method when serialization is used.
        /// </summary>
        public SerializationMethod SerializationMethod { get; set; } = SerializationMethod.ZeroFormat;

        #endregion

        #region CookedRabbit RabbitServices Configurable Settings

        /// <summary>
        /// Allows errors to be written to Console asynchornously. Will still write a simple message to console when ILogger is null and WriteErrorsToIlogger is true;
        /// </summary>
        public bool WriteErrorsToConsole { get; set; } = true;

        /// <summary>
        /// Allows errors to be passed to an ILogger. Will write to ILogger if it is null;
        /// </summary>
        public bool WriteErrorsToILogger { get; set; } = true;

        /// <summary>
        /// Allows PublishMany to throttle Rand(0,2) milliseconds between publish. Highly recommended to keep system responsiveness high.
        /// </summary>
        public bool ThrottleFastBodyLoops { get; set; } = true;

        /// <summary>
        /// On exception throw to calling methods.
        /// </summary>
        public bool ThrowExceptions { get; set; } = false;

        /// <summary>
        /// RabbitMQ consumer parameters.
        /// </summary>
        public ushort QosPrefetchSize { get; set; } = 0;

        /// <summary>
        /// RabbitMQ consumer parameters.
        /// </summary>
        public ushort QosPrefetchCount { get; set; } = 120;

        #endregion

        #region CookedRabbit Pool Settings

        /// <summary>
        /// Configures the await Task.Delay(x ms) when Pool is out of channels (temporarily).
        /// </summary>
        public ushort EmptyPoolWaitTime { get; set; } = 100;

        /// <summary>
        /// Value to configure the Connection display names in RabbitMQ HTTP API.
        /// </summary>
        public string ConnectionName { get; set; } = string.Empty;

        /// <summary>
        /// Number of connections to be created in the connection pool.
        /// </summary>
        public ushort ConnectionPoolCount { get; set; } = 4;

        /// <summary>
        /// Number of channels to keep in each of the channel pools.
        /// </summary>
        public ushort ChannelPoolCount { get; set; } = 100;

        #endregion

        #region RabbitMQ Connection Factory Settings

        /// <summary>
        /// Indicates to CookedRabbit to use Uri based connection string.
        /// </summary>
        public bool UseUri { get; set; } = false;

        /// <summary>
        /// Channel Factory (RabbitMQ) Uri connection string.
        /// <para>amqp(s) : user : password @ machinename : port / vhost</para>
        /// </summary>
        public Uri Uri { get; set; } = new Uri("amqp://guest:guest@localhost:5672/");

        /// <summary>
        /// Channel Factory (RabbitMQ) server user name. Used if UseUri is false.
        /// </summary>
        public string RabbitHostUser { get; set; } = "guest";

        /// <summary>
        /// Channel Factory (RabbitMQ) server user password. Used if UseUri is false.
        /// </summary>
        public string RabbitHostPassword { get; set; } = "guest";

        /// <summary>
        /// Channel Factory (RabbitMQ) server vhost. Used if UseUri is false.
        /// </summary>
        public string RabbitVHost { get; set; } = "/";

        /// <summary>
        /// Channel Factory (RabbitMQ) node hostname. Used if UseUri is false.
        /// </summary>
        public string RabbitHostName { get; set; } = "localhost";

        /// <summary>
        /// Channel Factory (RabbitMQ) node port number (5671 is SSL). Used if UseUri is false.
        /// </summary>
        public int RabbitPort { get; set; } = 5672;

        /// <summary>
        /// Channel Factory (RabbitMQ) max connection property.
        /// </summary>
        public ushort MaxChannelsPerConnection { get; set; } = 1000;

        /// <summary>
        /// Channel Factory (RabbitMQ) timespan (in seconds) between heartbeats. More than two timeouts in a row trigger RabbitMQ AutoRecovery.
        /// </summary>
        public ushort HeartbeatInterval { get; set; } = 6;

        /// <summary>
        /// Channel Factory (RabbitMQ) autorecovery property.
        /// </summary>
        public bool AutoRecovery { get; set; } = true;

        /// <summary>
        /// Channel Factory (RabbitMQ) topology recovery property.
        /// </summary>
        public bool TopologyRecovery { get; set; } = true;

        /// <summary>
        /// Channel Factory (RabbitMQ) net recovery property (seconds).
        /// </summary>
        public ushort NetRecoveryTimeout { get; set; } = 10;

        /// <summary>
        /// Channel Factory (RabbitMQ) specify the timeout on protocol operations (seconds).
        /// </summary>
        public ushort ContinuationTimeout { get; set; } = 10;

        /// <summary>
        /// Channel Factory (RabbitMQ) property to enable Async consumers. Can't be true and retrieve regular consumers.
        /// </summary>
        public bool EnableDispatchConsumersAsync { get; set; } = false;

        /// <summary>
        /// Channel Factory (RabbitMQ) specify the use of background threads for IO.
        /// <para>Becareful with this - it might have unintended side effects.</para>
        /// </summary>
        public bool UseBackgroundThreadsForIO { get; set; } = false;

        #endregion
    }
}
