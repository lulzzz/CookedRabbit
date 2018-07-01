namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// RabbitTopologySeasoning allows for slightly different default values for RabbitTopologyService settings.
    /// </summary>
    public class RabbitTopologySeasoning : RabbitSeasoning
    {
        #region RabbitTopologyService Overrides

        /// <summary>
        /// Configures the await Task.Delay(x ms) when Pool is out of channels (temporarily).
        /// </summary>
        public new ushort EmptyPoolWaitTime { get; set; } = 100;

        /// <summary>
        /// RabbitMQ node/url value.
        /// </summary>
        public new string RabbitHost { get; set; } = string.Empty;

        /// <summary>
        /// Value to configure the Connection display names in RabbitMQ HTTP API.
        /// </summary>
        public new string ConnectionName { get; set; } = string.Empty;

        /// <summary>
        /// Number of connections to be created in the connection pool.
        /// </summary>
        public new ushort ConnectionPoolCount { get; set; } = 2;

        /// <summary>
        /// Number of channels to keep in each of the channel pools.
        /// </summary>
        public new ushort ChannelPoolCount { get; set; } = 10;

        #endregion
    }
}
