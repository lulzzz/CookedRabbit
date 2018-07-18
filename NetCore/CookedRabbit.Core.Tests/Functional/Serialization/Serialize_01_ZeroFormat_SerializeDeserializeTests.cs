using AutoFixture;
using CookedRabbit.Core.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.Serialization;

namespace CookedRabbit.Core.Tests.Functional
{
    public class Serialize_01_ZeroFormat_SerializeDeserializeTests
    {
        [Fact]
        [Trait("Core Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
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
            await Assert.ThrowsAsync<InvalidOperationException>(() => SerializeAsZeroFormatAsync(testObject));
        }

        [Fact]
        [Trait("Core Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
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
        [Trait("Core Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
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
        [Trait("Core Serialize", "ZeroFormat NoCompression SerializeDeserialize")]
        public async Task SerializeDeserializeManyAsync()
        {
            // Arrange
            var rand = new Random();
            var fixture = new Fixture();
            var objectCount = 13;
            var objects = fixture.CreateMany<ZeroTestHelperObject>(objectCount).ToList();
            var serializedObjects = new List<byte[]>();
            var deserializedObjects = new List<ZeroTestHelperObject>();

            // Act
            for (int i = 0; i < objects.Count; i++)
            {
                serializedObjects.Add(await SerializeAsZeroFormatAsync(objects[i]));
            }

            foreach (var obj in serializedObjects)
            {
                deserializedObjects.Add(await DeserializeAsZeroFormatAsync<ZeroTestHelperObject>(obj));
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
    }
}
