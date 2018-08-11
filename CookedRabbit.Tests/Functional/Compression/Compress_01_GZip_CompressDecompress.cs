﻿using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static CookedRabbit.Library.Utilities.Compression;
using static CookedRabbit.Tests.Setup.GlobalVariables;

namespace CookedRabbit.Tests.Functional
{
    public class Compress_01_GZip_CompressDecompress
    {
        private readonly ITestOutputHelper _output;

        public Compress_01_GZip_CompressDecompress(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Net Compress", "GZip CompressDecompress")]
        public async Task CompressAsync()
        {
            // Act
            var data = Encoding.UTF8.GetBytes(SmallMessage);
            var compressedData = await CompressBytesWithGzipAsync(data);

            // Assert
            Assert.NotNull(data);
            Assert.True((compressedData.Length < data.Length), "Compressed data was bigger than non-compressed data");

            var str = Encoding.UTF8.GetString(compressedData);
            var ratio = ((data.Length - compressedData.Length) / (double)data.Length) * 100.0;
            _output.WriteLine($"Compression ratio for LZ4: {ratio.ToString("0.00000")}%");
        }

        [Fact]
        [Trait("Net Compress", "GZip CompressDecompress")]
        public async Task CompressDecompressAsync()
        {
            // Act
            var data = Encoding.UTF8.GetBytes(LargeMessage);
            var compressedData = await CompressBytesWithGzipAsync(data);

            // Assert
            Assert.NotNull(data);
            Assert.True((compressedData.Length < data.Length), "Compressed data was bigger than non-compressed data");

            var ratio = ((data.Length - compressedData.Length) / (double)data.Length) * 100.0;
            _output.WriteLine($"Compression ratio for LZ4: {ratio.ToString("0.00000")}%");

            // Re-Act
            var uncompressedData = await DecompressBytesWithGzipAsync(compressedData);
            var MessageUncompressed = Encoding.UTF8.GetString(uncompressedData);

            // Re-Assert
            Assert.True(data.Length == uncompressedData.Length, "Uncompressed data was not the same size as the original data.");
            Assert.Equal(LargeMessage, MessageUncompressed);
        }
    }
}
