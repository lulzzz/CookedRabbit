using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Utilities
{
    /// <summary>
    /// CookedRabbit Utility class to compress objects.
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Compresses a byte[] with Gzip.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<byte[]> CompressBytesWithGzipAsync(byte[] input)
        {
            byte[] output = null;

            using (MemoryStream memoryStream = new MemoryStream()) // Don't stack these usings, Gzip stream won't flush until dispose
            {
                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, false))
                { await gzipStream.WriteAsync(input, 0, input.Length); }

                output = memoryStream.ToArray();
            }

            return output;
        }

        /// <summary>
        /// Decompresses a byte[] with Gzip.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<byte[]> DecompressBytesWithGzipAsync(byte[] input)
        {
            byte[] output = null;

            using (MemoryStream outboundStream = new MemoryStream())
            {
                using (MemoryStream memoryStream = new MemoryStream(input))
                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress, false))
                {
                    await gzipStream.CopyToAsync(outboundStream);
                }

                output = outboundStream.ToArray();
            }

            return output;
        }
    }
}
