using LZ4;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Library.Utilities
{
    /// <summary>
    /// CookedRabbit Utility class to help compress objects.
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Compresses the byte[] with the optional compression method.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<byte[]> CompressAsync(byte[] input, CompressionMethod method = CompressionMethod.Gzip)
        {
            byte[] output = null;

            switch (method)
            {
                case CompressionMethod.Gzip:
                    output = await CompressBytesWithGzipAsync(input);
                    break;
                case CompressionMethod.Deflate:
                    output = await CompressBytesWithGzipAsync(input);
                    break;
                case CompressionMethod.LZ4:
                    output = await CompressBytesWithLZ4Async(input);
                    break;
                default: break;
            }

            return output;
        }

        /// <summary>
        /// Decompresses the byte[] with the optional compression method.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<byte[]> DecompressAsync(byte[] input, CompressionMethod method = CompressionMethod.Gzip)
        {
            byte[] output = null;

            switch (method)
            {
                case CompressionMethod.Gzip:
                    output = await DecompressBytesWithGzipAsync(input);
                    break;
                case CompressionMethod.Deflate:
                    output = await DecompressBytesWithGzipAsync(input);
                    break;
                case CompressionMethod.LZ4:
                    output = await DecompressBytesWithLZ4Async(input);
                    break;
                default: break;
            }

            return output;
        }

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

            using (MemoryStream outboundStream = new MemoryStream()) // Don't stack this using, Gzip stream won't flush until dispose
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

        /// <summary>
        /// Compresses a byte[] with Deflate.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<byte[]> CompressBytesWithDeflateAsync(byte[] input)
        {
            byte[] output = null;

            using (MemoryStream memoryStream = new MemoryStream()) // Don't stack these usings, Gzip stream won't flush until dispose
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, false))
                { await deflateStream.WriteAsync(input, 0, input.Length); }

                output = memoryStream.ToArray();
            }

            return output;
        }

        /// <summary>
        /// Decompresses a byte[] with Deflate.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<byte[]> DecompressBytesWithDeflateAsync(byte[] input)
        {
            byte[] output = null;

            using (MemoryStream outboundStream = new MemoryStream()) // Don't stack this using, Gzip stream won't flush until dispose
            {
                using (MemoryStream memoryStream = new MemoryStream(input))
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, false))
                {
                    await deflateStream.CopyToAsync(outboundStream);
                }

                output = outboundStream.ToArray();
            }

            return output;
        }

        /// <summary>
        /// Compresses a byte[] with Deflate.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<byte[]> CompressBytesWithLZ4Async(byte[] input)
        {
            byte[] output = null;

            using (MemoryStream memoryStream = new MemoryStream()) // Don't stack these usings, Gzip stream won't flush until dispose
            {
                using (LZ4Stream lz4Stream = new LZ4Stream(memoryStream, LZ4StreamMode.Compress, LZ4StreamFlags.Default))
                { await lz4Stream.WriteAsync(input, 0, input.Length); }

                output = memoryStream.ToArray();
            }

            return output;
        }

        /// <summary>
        /// Decompresses a byte[] with Deflate.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<byte[]> DecompressBytesWithLZ4Async(byte[] input)
        {
            byte[] output = null;

            using (MemoryStream outboundStream = new MemoryStream()) // Don't stack this using, Gzip stream won't flush until dispose
            {
                using (MemoryStream memoryStream = new MemoryStream(input))
                using (LZ4Stream lz4Stream = new LZ4Stream(memoryStream, LZ4StreamMode.Decompress, LZ4StreamFlags.Default))
                {
                    await lz4Stream.CopyToAsync(outboundStream);
                }

                output = outboundStream.ToArray();
            }

            return output;
        }
    }
}
