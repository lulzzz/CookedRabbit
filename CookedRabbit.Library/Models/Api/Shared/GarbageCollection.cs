using System.Runtime.Serialization;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for Garbage Collection information.
    /// </summary>
    public class GarbageCollection
    {
        /// <summary>
        /// Minor GCS
        /// </summary>
        [DataMember(Name = "minor_gcs")]
        public int MinorGCS { get; set; }
        /// <summary>
        /// Fullsweep After
        /// </summary>
        [DataMember(Name = "fullsweep_after")]
        public int FullSweepAfter { get; set; }
        /// <summary>
        /// Minimum Heap Size
        /// </summary>
        [DataMember(Name = "min_heap_size")]
        public int MinHeapSize { get; set; }
        /// <summary>
        /// Minimum Binary Virtual Heap Size
        /// </summary>
        [DataMember(Name = "min_bin_vheap_size")]
        public int MinBinVheapSize { get; set; }
        /// <summary>
        /// Max Heap Size
        /// </summary>
        [DataMember(Name = "max_heap_size")]
        public int MaxHeapSize { get; set; }
    }
}
