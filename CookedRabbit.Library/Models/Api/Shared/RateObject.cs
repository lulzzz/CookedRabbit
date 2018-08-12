using System.Runtime.Serialization;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for Rate information.
    /// </summary>
    public class RateObject
    {
        /// <summary>
        /// Rate
        /// </summary>
        [DataMember(Name = "rate")]
        public double Rate { get; set; }
    }
}
