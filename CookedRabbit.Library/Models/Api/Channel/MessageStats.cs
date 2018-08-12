using System.Runtime.Serialization;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for Channel Message Stat information.
    /// </summary>
    public class MessageStats
    {
        /// <summary>
        /// DeliverGetDetails
        /// </summary>
        [DataMember(Name = "deliver_get_details")]
        public RateObject DeliverGetDetails { get; set; }
        /// <summary>
        /// DeliverGet
        /// </summary>
        [DataMember(Name = "deliver_get")]
        public int DeliverGet { get; set; }
        /// <summary>
        /// AckDetails
        /// </summary>
        [DataMember(Name = "ack_details")]
        public RateObject AckDetails { get; set; }
        /// <summary>
        /// Vhost
        /// </summary>
        [DataMember(Name = "ack")]
        public int Ack { get; set; }
        /// <summary>
        /// RedeliverDetails
        /// </summary>
        [DataMember(Name = "redeliver_details")]
        public RateObject RedeliverDetails { get; set; }
        /// <summary>
        /// Redeliver
        /// </summary>
        [DataMember(Name = "redeliver")]
        public int Redeliver { get; set; }
        /// <summary>
        /// DeliverNoAckDetails
        /// </summary>
        [DataMember(Name = "deliver_no_ack_details")]
        public RateObject DeliverNoAckDetails { get; set; }
        /// <summary>
        /// DeliverNoAck
        /// </summary>
        [DataMember(Name = "deliver_no_ack")]
        public int DeliverNoAck { get; set; }
        /// <summary>
        /// DeliverDetails
        /// </summary>
        [DataMember(Name = "deliver_details")]
        public RateObject DeliverDetails { get; set; }
        /// <summary>
        /// Deliver
        /// </summary>
        [DataMember(Name = "deliver")]
        public int Deliver { get; set; }
        /// <summary>
        /// GetNoAckDetails
        /// </summary>
        [DataMember(Name = "get_no_ack_details")]
        public RateObject GetNoAckDetails { get; set; }
        /// <summary>
        /// GetNoAck
        /// </summary>
        [DataMember(Name = "GetNoAck")]
        public int GetNoAck { get; set; }
        /// <summary>
        /// GetNoAckDetails
        /// </summary>
        [DataMember(Name = "get_details")]
        public RateObject GetDetails { get; set; }
        /// <summary>
        /// Get
        /// </summary>
        [DataMember(Name = "get")]
        public int Get { get; set; }
    }
}
