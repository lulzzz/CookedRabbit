using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// CookedRabbit Envelope to simplify publishing options.
    /// </summary>
    public class Envelope
    {
        /// <summary>
        /// Exchange name to publish to. Optional.
        /// </summary>
        public string ExchangeName { get; set; } = string.Empty;

        /// <summary>
        /// Routing key (for an exchange) or a queue name.
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Data to be published.
        /// </summary>
        public byte[] MessageBody { get; set; } = null;

        /// <summary>
        /// RabbitMQ mandatory option.
        /// </summary>
        public bool Mandatory { get; set; } = false;

        /// <summary>
        /// Used to adjust Message Properties.
        /// </summary>
        public ContentEncoding ContentEncoding { get; set; } = ContentEncoding.Binary;

        /// <summary>
        /// Used to adjust Message Properties.
        /// </summary>
        public string MessageType { get; set; } = $"{ContentType.Json} {Charset.Utf8}";

        /// <summary>
        /// Allows an envelope to quickly create copies of itself.
        /// </summary>
        /// <returns></returns>
        public Envelope Clone()
        {
            return (Envelope)MemberwiseClone();
        }
    }
}
