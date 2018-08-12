using System;
using System.Runtime.Serialization;

namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for Channel information.
    /// </summary>
    public class Channel
    {
        // Admin Data
        /// <summary>
        /// User
        /// </summary>
        [DataMember(Name = "user")]
        public string User { get; set; }
        /// <summary>
        /// User who performed action.
        /// </summary>
        [DataMember(Name = "user_who_performed_action")]
        public string ActionPerformedBy { get; set; }
        /// <summary>
        /// Node
        /// </summary>
        [DataMember(Name = "node")]
        public string Node { get; set; }
        /// <summary>
        /// Vhost
        /// </summary>
        [DataMember(Name = "vhost")]
        public string Vhost { get; set; }
        /// <summary>
        /// ChannelIdleSince
        /// </summary>
        [DataMember(Name = "idle_since")]
        public string ChannelIdleSince { get; set; }
        /// <summary>
        /// ChannelIdleSince DateTime
        /// </summary>
        [IgnoreDataMember]
        public DateTime ChannelIdleSinceDateTime
        {
            get { DateTime.TryParse(ChannelIdleSince, out DateTime dateTime); return dateTime; }
        }
        /// <summary>
        /// ReductionsDetails
        /// </summary>
        [DataMember(Name = "reductions_details")]
        public RateObject ReductionsDetails { get; set; }
        /// <summary>
        /// Reductions
        /// </summary>
        [DataMember(Name = "reductions")]
        public int Reductions { get; set; }


        // Channel Data
        /// <summary>
        /// ServerChannelNumber (not the same as the internal CookedRabbit ChannelId).
        /// </summary>
        [DataMember(Name = "number")]
        public int ServerChannelNumber { get; set; }
        /// <summary>
        /// ServerChannelName (not the same as the internal CookedRabbit ChannelId).
        /// </summary>
        [DataMember(Name = "name")]
        public string ServerChannelName { get; set; }
        /// <summary>
        /// State
        /// </summary>
        [DataMember(Name = "state")]
        public string State { get; set; }
        /// <summary>
        /// PrefectCount
        /// </summary>
        [DataMember(Name = "global_prefetch_count")]
        public int GlobalPrefectCount { get; set; }
        /// <summary>
        /// PrefectCount
        /// </summary>
        [DataMember(Name = "prefetch_count")]
        public int PrefectCount { get; set; }
        /// <summary>
        /// AcksUncommitted
        /// </summary>
        [DataMember(Name = "acks_uncommitted")]
        public int AcksUncommitted { get; set; }
        /// <summary>
        /// MessagesUncomitted
        /// </summary>
        [DataMember(Name = "messages_uncommitted")]
        public int MessagesUncomitted { get; set; }
        /// <summary>
        /// MessagesUnconfirmed
        /// </summary>
        [DataMember(Name = "messages_unconfirmed")]
        public int MessagesUnconfirmed { get; set; }
        /// <summary>
        /// MessagesUnacknowledged
        /// </summary>
        [DataMember(Name = "messages_unacknowledged")]
        public int MessagesUnacknowledged { get; set; }
        /// <summary>
        /// ConsumerCount
        /// </summary>
        [DataMember(Name = "consumer_count")]
        public int ConsumerCount { get; set; }
        /// <summary>
        /// Confirm
        /// </summary>
        [DataMember(Name = "confirm")]
        public bool Confirm { get; set; }
        /// <summary>
        /// Transactional
        /// </summary>
        [DataMember(Name = "transactional")]
        public bool Transactional { get; set; }


        // Technical Data
        /// <summary>
        /// Connection Details
        /// </summary>
        [DataMember(Name = "connection_details")]
        public ConnectionDetails ConnectionDetails { get; set; }
        /// <summary>
        /// Garbage Collection Details
        /// </summary>
        [DataMember(Name = "garbage_collection")]
        public GarbageCollection GarbageCollection { get; set; }
        /// <summary>
        /// Message Stats
        /// </summary>
        [DataMember(Name = "message_stats")]
        public MessageStats MessageStats { get; set; }
    }
}
