using System;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// Class to fully season RabbitServices to your taste!
    /// </summary>
    public class FactorySeasoning
    {
        #region RabbitMQ Connection Factory Settings

        /// <summary>
        /// Indicates to CookedRabbit to use Uri based connection string.
        /// </summary>
        public bool UseUri { get; set; } = true;

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
        /// Channel Factory (RabbitMQ) the amount of time to wait before netrecovery begins (seconds).
        /// </summary>
        public ushort NetRecoveryTimeout { get; set; } = 10;

        /// <summary>
        /// Channel Factory (RabbitMQ) specify the amount of time before timeout on protocol operations (seconds).
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
