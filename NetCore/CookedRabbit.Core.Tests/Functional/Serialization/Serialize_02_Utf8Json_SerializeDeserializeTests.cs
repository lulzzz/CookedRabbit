using AutoFixture;
using CookedRabbit.Core.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.Compression;
using static CookedRabbit.Core.Library.Utilities.Serialization;

namespace CookedRabbit.Core.Tests.Functional
{
    public class Serialize_02_Utf8Json_SerializeDeserializeTests
    {
        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression SerializeDeserialize")]
        public async Task SerializeAsync()
        {
            // Arrange
            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            // Act
            var data = await SerializeAsUtf8JsonFormatAsync(testObject);
            data = await CompressBytesWithLZ4Async(data);

            // Assert
            Assert.NotNull(data);
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression SerializeDeserialize")]
        public async Task SerializeAsJsonAsync()
        {
            // Arrange
            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            // Act
            var data = await SerializeToJsonAsync(testObject);
            var compressedData = await CompressBytesWithLZ4Async(Encoding.UTF8.GetBytes(data));

            // Assert
            Assert.NotNull(compressedData);
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression SerializeDeserialize")]
        public async Task SerializeDeserializeAsync()
        {
            // Arrange
            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            // Act
            var data = await SerializeAsUtf8JsonFormatAsync(testObject);
            data = await CompressBytesWithLZ4Async(data);

            var testData = await DecompressBytesWithLZ4Async(data);
            var desTestObject = await DeserializeAsUtf8JsonFormatAsync<ZeroTestHelperObject>(testData);

            // Re-Assert
            Assert.NotNull(data);
            Assert.NotNull(testData);
            Assert.NotNull(desTestObject);
            Assert.Equal(testObject.Name, desTestObject.Name);
            Assert.Equal(testObject.Address, desTestObject.Address);
            Assert.Equal(testObject.BitsAndPieces, desTestObject.BitsAndPieces);
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression SerializeDeserialize")]
        public async Task SerializeAsJsonAndDeserializeAsync()
        {
            // Arrange
            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            // Act
            var jsonString = await SerializeToJsonAsync(testObject);
            var compressedData = await CompressBytesWithLZ4Async(Encoding.UTF8.GetBytes(jsonString));
            var decompressedString = Encoding.UTF8.GetString(await DecompressBytesWithLZ4Async(compressedData));
            var decompressedTestObject = await DeserializeJsonAsync<TestHelperObject>(decompressedString);


            // Assert
            Assert.NotNull(jsonString);
            Assert.NotNull(decompressedString);
            Assert.NotNull(decompressedTestObject);
            Assert.Equal(testObject.Name, decompressedTestObject.Name);
            Assert.Equal(testObject.Address, decompressedTestObject.Address);
            Assert.Equal(testObject.BitsAndPieces, decompressedTestObject.BitsAndPieces);
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression SerializeDeserialize")]
        public async Task SerializeDeserializeManyAsync()
        {
            // Arrange
            var rand = new Random();
            var fixture = new Fixture();
            var objectCount = 13;
            var objects = fixture.CreateMany<TestHelperObject>(objectCount).ToList();
            var serializedObjects = new List<byte[]>();
            var deserializedObjects = new List<TestHelperObject>();

            // Act
            for (int i = 0; i < objects.Count; i++)
            {
                var data = await SerializeAsUtf8JsonFormatAsync(objects[i]);
                data = await CompressBytesWithLZ4Async(data);
                serializedObjects.Add(data);
            }

            foreach (var obj in serializedObjects)
            {
                var testData = await DecompressBytesWithLZ4Async(obj);
                var testObj = await DeserializeAsUtf8JsonFormatAsync<TestHelperObject>(testData);
                deserializedObjects.Add(testObj);
            }

            // Assert
            Assert.True(serializedObjects.Count == objectCount, "Lost objects during serialization.");
            Assert.True(deserializedObjects.Count == objectCount, "Lost objects during deserialization.");

            // Deep Assert - Check 100% Equality
            for (int i = 0; i < deserializedObjects.Count; i++)
            {
                Assert.Equal(deserializedObjects.ElementAt(i).Name, objects.ElementAt(i).Name);
                Assert.Equal(deserializedObjects.ElementAt(i).Address, objects.ElementAt(i).Address);
                Assert.Equal(deserializedObjects.ElementAt(i).BitsAndPieces, objects.ElementAt(i).BitsAndPieces);
            }
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression SerializeDeserialize")]
        public async Task SerializeDeserializeManyAsJsonAsync()
        {
            // Arrange
            var rand = new Random();
            var fixture = new Fixture();
            var objectCount = 13;
            var objects = fixture.CreateMany<TestHelperObject>(objectCount).ToList();
            var compressedObjects = new List<byte[]>();
            var deserializedObjects = new List<TestHelperObject>();

            // Act
            for (int i = 0; i < objects.Count; i++)
            {
                var jsonString = await SerializeToJsonAsync(objects[i]);
                var compressedData = await CompressBytesWithLZ4Async(Encoding.UTF8.GetBytes(jsonString));
                compressedObjects.Add(compressedData);
            }

            foreach (var obj in compressedObjects)
            {
                var decompressedString = Encoding.UTF8.GetString(await DecompressBytesWithLZ4Async(obj));
                deserializedObjects.Add(await DeserializeJsonAsync<TestHelperObject>(decompressedString));
            }

            // Assert
            Assert.True(compressedObjects.Count == objectCount, "Lost objects during serialization.");
            Assert.True(deserializedObjects.Count == objectCount, "Lost objects during deserialization.");

            // Deep Assert - Check 100% Equality
            for (int i = 0; i < deserializedObjects.Count; i++)
            {
                Assert.Equal(deserializedObjects.ElementAt(i).Name, objects.ElementAt(i).Name);
                Assert.Equal(deserializedObjects.ElementAt(i).Address, objects.ElementAt(i).Address);
                Assert.Equal(deserializedObjects.ElementAt(i).BitsAndPieces, objects.ElementAt(i).BitsAndPieces);
            }
        }
    }
}
