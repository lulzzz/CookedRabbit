using AutoFixture;
using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Tests.Fixtures;
using CookedRabbit.Core.Tests.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Tests.Integration
{
    [Collection("IntegrationTests_Compression_Utf8")]
    public class Serialize_02_Utf8Json_Compression_PublishGetTests
    {
        private readonly Utf8_Compression _fixture;

        public Serialize_02_Utf8Json_Compression_PublishGetTests(Utf8_Compression fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json NoCompression PublishGet")]
        public async Task PublishAndGetAsync()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName2}.3112";
            var exchangeName = string.Empty;

            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative" , "spraytan", "traitor", "peetape", "kompromat" }
            };

            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitSerializeService.SerializeAndPublishAsync(testObject, envelope);
            await Task.Delay(200); // Allow Server Side Routing
            var result = await _fixture.RabbitSerializeService.GetAndDeserializeAsync<TestHelperObject>(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");
            Assert.True(deleteSuccess);
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json NoCompression PublishGet")]
        public async Task PublishManyAndGetManyAsync()
        {
            var fixture = new Fixture();

            // Arrange
            var messageCount = 17;
            var messages = fixture.CreateMany<TestHelperObject>(messageCount).ToList();
            var queueName = $"{_fixture.TestQueueName3}.3113";
            var exchangeName = string.Empty;
            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _fixture.RabbitSerializeService.SerializeAndPublishManyAsync(messages, envelope);
            await Task.Delay(200); // Allow Server Side Routing
            var results = await _fixture.RabbitSerializeService.GetAndDeserializeManyAsync<TestHelperObject>(queueName, messageCount);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(results.Count == messageCount, "Messages were lost.");
            Assert.True(deleteSuccess);
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression PublishGet")]
        public async Task CompressPublishAndGetAsync()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName2}.3112";
            var exchangeName = string.Empty;

            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitSerializeService.SerializeAndPublishAsync(testObject, envelope);
            await Task.Delay(200); // Allow Server Side Routing
            var result = await _fixture.RabbitSerializeService.GetAndDeserializeAsync<TestHelperObject>(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");
            Assert.True(deleteSuccess);
        }

        [Fact]
        [Trait("NetCore Serialize", "Utf8Json Compression PublishGet")]
        public async Task CompressPublishManyAndGetManyAsync()
        {
            var fixture = new Fixture();

            // Arrange
            var messageCount = 17;
            var messages = fixture.CreateMany<TestHelperObject>(messageCount).ToList();
            var queueName = $"{_fixture.TestQueueName3}.3113";
            var exchangeName = string.Empty;
            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _fixture.RabbitSerializeService.SerializeAndPublishManyAsync(messages, envelope);
            await Task.Delay(200); // Allow Server Side Routing
            var results = await _fixture.RabbitSerializeService.GetAndDeserializeManyAsync<TestHelperObject>(queueName, messageCount);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(results.Count == messageCount, "Messages were lost.");
            Assert.True(deleteSuccess);
        }
    }
}
