using System.Runtime.Serialization;

namespace CookedRabbit.Library.Models
{
    public class Capabilities
    {
        [DataMember(Name = "authentication_failure_close")]
        public bool AuthenticationFailureClose { get; set; }
        [DataMember(Name = "basic.nack")]
        public bool BasicNack { get; set; }
        [DataMember(Name = "connection.blocked")]
        public bool ConnectionBlocked { get; set; }
        [DataMember(Name = "consumer_cancel_notify")]
        public bool ConsumerCancelNotify { get; set; }
        [DataMember(Name = "exchange_exchange_bindings")]
        public bool ExchangeToExchangeBindings { get; set; }
        [DataMember(Name = "publisher_confirms")]
        public bool PublisherConfirms { get; set; }
    }
}
