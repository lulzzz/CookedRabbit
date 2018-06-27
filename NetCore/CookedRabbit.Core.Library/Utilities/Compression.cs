using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Utilities
{
    public static class Compression
    {
        public static async Task<byte[]> CompressBytesWithGzipAsync(byte[] input)
        {
            byte[] output = null;

            using (MemoryStream memoryStream = new MemoryStream())
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, false))
            {
                await gzipStream.WriteAsync(input, 0, input.Length);
                output = memoryStream.ToArray();
            }

            return output;
        }

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
