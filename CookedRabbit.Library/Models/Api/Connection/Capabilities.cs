using System.Runtime.Serialization;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for Capabilities.
    /// </summary>
    public class Capabilities
    {
        /// <summary>
        /// AuthenticationFailureClose
        /// </summary>
        [DataMember(Name = "authentication_failure_close")]
        public bool AuthenticationFailureClose { get; set; }
        /// <summary>
        /// BasicNack
        /// </summary>
        [DataMember(Name = "basic.nack")]
        public bool BasicNack { get; set; }
        /// <summary>
        /// ConnectionBlocked
        /// </summary>
        [DataMember(Name = "connection.blocked")]
        public bool ConnectionBlocked { get; set; }
        /// <summary>
        /// ConsumerCancelNotify
        /// </summary>
        [DataMember(Name = "consumer_cancel_notify")]
        public bool ConsumerCancelNotify { get; set; }
        /// <summary>
        /// ExchangeToExchange Bindings
        /// </summary>
        [DataMember(Name = "exchange_exchange_bindings")]
        public bool ExchangeToExchangeBindings { get; set; }
        /// <summary>
        /// PublisherConfirms
        /// </summary>
        [DataMember(Name = "publisher_confirms")]
        public bool PublisherConfirms { get; set; }
    }
}
