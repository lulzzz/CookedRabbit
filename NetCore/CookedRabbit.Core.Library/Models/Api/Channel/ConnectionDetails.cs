using System.Runtime.Serialization;

namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for Channel ConnectionDetails information.
    /// </summary>
    public class ConnectionDetails
    {
        /// <summary>
        /// Peer Host
        /// </summary>
        [DataMember(Name = "peer_host")]
        public string PeerHost { get; set; }
        /// <summary>
        /// Peer Port
        /// </summary>
        [DataMember(Name = "peer_port")]
        public int PeerPort { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
