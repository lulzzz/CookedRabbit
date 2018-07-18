namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// Class to fully season RabbitServices to your taste!
    /// </summary>
    public class RabbitSeasoning
    {
        #region CookedRabbit Global Settings

        /// <summary>
        /// Allows errors to be written to Console asynchornously. Will still write a simple message to console when ILogger is null and WriteErrorsToIlogger is true;
        /// </summary>
        public bool WriteErrorsToConsole { get; set; } = false;

        /// <summary>
        /// Allows errors to be passed to an ILogger. Will write to ILogger if it is null;
        /// </summary>
        public bool WriteErrorsToILogger { get; set; } = true;

        /// <summary>
        /// Allows any Publish that uses a loop to throttle Rand(0,2) milliseconds between publish.
        /// <para>Highly recommended on multi-threaded systems to keep total system responsiveness high at the cost of individual thread throughput.</para>
        /// </summary>
        public bool ThrottleFastBodyLoops { get; set; } = true;

        /// <summary>
        /// On exception, throw to calling methods.
        /// </summary>
        public bool ThrowExceptions { get; set; } = false;

        /// <summary>
        /// On exception, break any batch operation. Recommended but not necessary. Superceded by ThrowExceptions.
        /// </summary>
        public bool BatchBreakOnException { get; set; } = true;

        /// <summary>
        /// RabbitMQ consumer parameters.
        /// </summary>
        public ushort QosPrefetchSize { get; set; } = 0;

        /// <summary>
        /// RabbitMQ consumer parameters.
        /// <para>To fine tune, check consumer utilization located in RabbitMQ HTTP API management.</para>
        /// </summary>
        public ushort QosPrefetchCount { get; set; } = 120;

        #endregion

        /// <summary>
        /// Class to hold settings for Serialization.
        /// </summary>
        public SerializationSeasoning SerializeSettings { get; set; } = new SerializationSeasoning();

        /// <summary>
        /// Class to hold settings for Channel/Connection pools.
        /// </summary>
        public PoolSeasoning PoolSettings { get; set; } = new PoolSeasoning();

        /// <summary>
        /// Class to hold settings for ChannelFactory (RabbitMQ) settings.
        /// </summary>
        public FactorySeasoning FactorySettings { get; set; } = new FactorySeasoning();

        /// <summary>
        /// Class to hold settings for ChannelFactory/SSL (RabbitMQ) settings.
        /// </summary>
        public SslSeasoning SslSettings { get; set; } = new SslSeasoning();
    }
}
