using System.Runtime.Serialization;

namespace CookedRabbit.Core.Library.Models
{
    public class ClientProperties
    {
        [DataMember(Name = "capabilities")]
        public Capabilities Capabilities { get; set; }
        [DataMember(Name = "connection_name")]
        public string ConnectionName { get; set; }
        [DataMember(Name = "copyright")]
        public string Copyright { get; set; }
        [DataMember(Name = "information")]
        public string Information { get; set; }
        [DataMember(Name = "platform")]
        public string Platform { get; set; }
        [DataMember(Name = "product")]
        public string Product { get; set; }
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}
