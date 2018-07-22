using System.Threading;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// Class to fully season RabbitServices to your taste!
    /// </summary>
    public class PoolSeasoning
    {
        #region CookedRabbit Pool Settings

        /// <summary>
        /// Value to configure the Connection display names in RabbitMQ HTTP API.
        /// </summary>
        public string ConnectionName { get; set; } = string.Empty;

        /// <summary>
        /// Number of connections to be created in the connection pool. Used in round-robin to create channels.
        /// </summary>
        public ushort ConnectionPoolCount { get; set; } = 5;

        /// <summary>
        /// Number of channels to keep in each of the channel pools. Used in round-robin to perform actions.
        /// </summary>
        public ushort ChannelPoolCount { get; set; } = 25;

        #region AutoScale Settings

        private int _enableAutoScaling = 0;

        /// <summary>
        /// Thread safe boolean to enable/disable AutoScaling.
        /// <para>When enabled, a thead safe hysteresis check is used to slowly add channels to the pool to relieve non-available channel congestion.</para>
        /// </summary>
        public bool EnableAutoScaling
        {
            get
            { return (Interlocked.CompareExchange(ref _enableAutoScaling, 1, 1) == 1); }
            set
            {
                if (value)
                { Interlocked.CompareExchange(ref _enableAutoScaling, 1, 0); }
                else
                { Interlocked.CompareExchange(ref _enableAutoScaling, 0, 1); }
            }
        }

        /// <summary>
        /// Configures the await Task.Delay(x ms) when Pool is out of channels (temporarily).
        /// </summary>
        public ushort EmptyPoolWaitTime { get; set; } = 100;

        /// <summary>
        /// Increase channels upon reaching the wait scaling threshold.
        /// </summary>
        public int ScalingWaitThreshold { get; set; } = 3;

        /// <summary>
        /// Writes when the channel pool sleeps to console.
        /// </summary>
        public bool WriteSleepNoticeToConsole { get; set; } = false;

        /// <summary>
        /// Maximum channels that auto scale can add.
        /// </summary>
        public long MaxAutoScaleChannels { get; set; } = 100;

        #endregion

        #endregion
    }
}
