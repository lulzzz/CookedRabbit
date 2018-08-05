using System.Runtime.Serialization;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for ClientProperties.
    /// </summary>
    public class ClientProperties
    {
        /// <summary>
        /// Capabilities
        /// </summary>
        [DataMember(Name = "capabilities")]
        public Capabilities Capabilities { get; set; }
        /// <summary>
        /// ConnectionName
        /// </summary>
        [DataMember(Name = "connection_name")]
        public string ConnectionName { get; set; }
        /// <summary>
        /// Copyright
        /// </summary>
        [DataMember(Name = "copyright")]
        public string Copyright { get; set; }
        /// <summary>
        /// Information
        /// </summary>
        [DataMember(Name = "information")]
        public string Information { get; set; }
        /// <summary>
        /// Platform
        /// </summary>
        [DataMember(Name = "platform")]
        public string Platform { get; set; }
        /// <summary>
        /// Product
        /// </summary>
        [DataMember(Name = "product")]
        public string Product { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}
