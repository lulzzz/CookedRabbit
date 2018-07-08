using AutoFixture;
using CookedRabbit.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Serialization;

namespace CookedRabbit.Tests.Integrations
{
    public class Serialize_01_ZeroFormat_SerializeDeserializeTests
    {
        [Fact]
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
        public async Task SerializeExceptionAsync()
        {
            // Arrange
            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            // Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(() => SerializeAsZeroFormatAsync(testObject));
        }

        [Fact]
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
        public async Task SerializeAsync()
        {
            // Arrange
            var testObject = new ZeroTestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            // Act
            var data = await SerializeAsZeroFormatAsync(testObject);

            // Assert
            Assert.NotNull(data);
        }

        [Fact]
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
        public async Task SerializeDeserializeAsync()
        {
            // Arrange
            var testObject = new ZeroTestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            // Act
            var data = await SerializeAsZeroFormatAsync(testObject);
            var desTestObject = await DeserializeAsZeroFormatAsync<ZeroTestHelperObject>(data);

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(desTestObject);
            Assert.Equal(testObject.Name, desTestObject.Name);
            Assert.Equal(testObject.Address, desTestObject.Address);
            Assert.Equal(testObject.BitsAndPieces, desTestObject.BitsAndPieces);
        }

        [Fact]
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
        public async Task SerializeDeserializeManyAsync()
        {
            // Arrange
            var rand = new Random();
            var fixture = new Fixture();
            var objectCount = 17;
            var objects = fixture.CreateMany<ZeroTestHelperObject>(objectCount).ToList();
            var serializedObjects = new List<byte[]>();
            var deserializedObjects = new List<ZeroTestHelperObject>();
            var index = rand.Next(0, objectCount - 1);

            // Act
            for (int i = 0; i < objects.Count; i++)
            {
                serializedObjects.Add(await SerializeAsZeroFormatAsync(objects[i]));
            }

            foreach (var obj in serializedObjects)
            {
                deserializedObjects.Add(await DeserializeAsZeroFormatAsync<ZeroTestHelperObject>(obj));
            }

            // Arrange
            Assert.True(serializedObjects.Count == objectCount, "Lost objects during serialization.");
            Assert.True(deserializedObjects.Count == objectCount, "Lost objects during deserialization.");

            // Re-Arrange Spot Check Equality
            Assert.Equal(deserializedObjects.ElementAt(index).BitsAndPieces, objects.ElementAt(index).BitsAndPieces);

            index = rand.Next(0, objectCount - 1);
            Assert.Equal(deserializedObjects.ElementAt(index).BitsAndPieces, objects.ElementAt(index).BitsAndPieces);

            index = rand.Next(0, objectCount - 1);
            Assert.Equal(deserializedObjects.ElementAt(index).BitsAndPieces, objects.ElementAt(index).BitsAndPieces);
        }
    }
}
