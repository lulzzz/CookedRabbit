using AutoFixture;
using CookedRabbit.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Serialization;

namespace CookedRabbit.Tests.Functional
{
    public class Serialize_01_Utf8Json_SerializeDeserializeTests
    {
        [Fact]
        [Trait("Rabbit Serialize", "Utf8Json NoCompression SerializeDeserialize")]
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

            // Assert
            Assert.NotNull(data);
        }

        [Fact]
        [Trait("Rabbit Serialize", "Utf8Json NoCompression SerializeDeserialize")]
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

            // Assert
            Assert.NotNull(data);
        }

        [Fact]
        [Trait("Rabbit Serialize", "Utf8Json NoCompression SerializeDeserialize")]
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
            var desTestObject = await DeserializeAsUtf8JsonFormatAsync<TestHelperObject>(data);

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(desTestObject);
            Assert.Equal(testObject.Name, desTestObject.Name);
            Assert.Equal(testObject.Address, desTestObject.Address);
            Assert.Equal(testObject.BitsAndPieces, desTestObject.BitsAndPieces);
        }

        [Fact]
        [Trait("Rabbit Serialize", "Utf8Json NoCompression SerializeDeserialize")]
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
            var data = await SerializeToJsonAsync(testObject);
            var desTestObject = await DeserializeJsonAsync<TestHelperObject>(data);

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(desTestObject);
            Assert.Equal(testObject.Name, desTestObject.Name);
            Assert.Equal(testObject.Address, desTestObject.Address);
            Assert.Equal(testObject.BitsAndPieces, desTestObject.BitsAndPieces);
        }

        [Fact]
        [Trait("Rabbit Serialize", "Utf8Json NoCompression SerializeDeserialize")]
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
                serializedObjects.Add(await SerializeAsUtf8JsonFormatAsync(objects[i]));
            }

            foreach (var obj in serializedObjects)
            {
                deserializedObjects.Add(await DeserializeAsUtf8JsonFormatAsync<TestHelperObject>(obj));
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
        [Trait("Rabbit Serialize", "Utf8Json NoCompression SerializeDeserialize")]
        public async Task SerializeDeserializeManyAsJsonAsync()
        {
            // Arrange
            var rand = new Random();
            var fixture = new Fixture();
            var objectCount = 13;
            var objects = fixture.CreateMany<TestHelperObject>(objectCount).ToList();
            var serializedObjects = new List<string>();
            var deserializedObjects = new List<TestHelperObject>();

            // Act
            for (int i = 0; i < objects.Count; i++)
            {
                serializedObjects.Add(await SerializeToJsonAsync(objects[i]));
            }

            foreach (var obj in serializedObjects)
            {
                deserializedObjects.Add(await DeserializeJsonAsync<TestHelperObject>(obj));
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
