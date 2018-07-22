using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// Class to fully season RabbitServices to your taste!
    /// </summary>
    public class SerializationSeasoning
    {
        #region CookedRabbit Serialization Settings

        /// <summary>
        /// Enables the use of compression and decompression.
        /// </summary>
        public bool CompressionEnabled { get; set; } = true;

        /// <summary>
        /// Sets the compression method when compression is used.
        /// </summary>
        public CompressionMethod CompressionMethod { get; set; } = CompressionMethod.LZ4;

        /// <summary>
        /// Sets the serialization method when serialization is used.
        /// </summary>
        public SerializationMethod SerializationMethod { get; set; } = SerializationMethod.Utf8Json;

        #endregion
    }
}
