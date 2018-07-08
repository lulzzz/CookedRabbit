using RabbitMQ.Client;
using System.Collections.Generic;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// CookedRabbit AckableResult returns both the RabbitMQ IModel (channel) and a list of BasicGetResuilt.
    /// </summary>
    public class AckableResult
    {
        /// <summary>
        /// RabbitMQ IModel (channel) for further action by caller (eg. channel.BasicAck(), .BasicNack(), etc.)
        /// </summary>
        public IModel Channel { get; set; }

        /// <summary>
        /// List of BasciGetResults for processing before manual Acknowledge/Rejection on channel.
        /// </summary>
        public List<BasicGetResult> Results { get; set; }
    }
}
