using CookedRabbit.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integration
{
    [Collection("IntegrationTests_Zero")]
    public class Delivery_02_PublishGetTests
    {
        private readonly IntegrationFixture_Zero _fixture;

        public Delivery_02_PublishGetTests(IntegrationFixture_Zero fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishAndGetAsync()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName1}.2111";
            var exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);
            var result = await _fixture.RabbitDeliveryService.GetAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishManyAndGetManyAsync()
        {
            // Arrange
            var messageCount = 17;
            var queueName = $"{_fixture.TestQueueName2}.2222";
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messageCount);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _fixture.RabbitDeliveryService.PublishManyAsync(exchangeName, queueName, payloads, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var queueCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var results = await _fixture.RabbitDeliveryService.GetManyAsync(queueName, messageCount);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(queueCount == messageCount, "Messages were lost in routing.");
            Assert.True(results.Count == messageCount);
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishManyAsBatchesAsync()
        {
            // Arrange
            var messageCount = 111;
            var queueName = $"{_fixture.TestQueueName3}.2333";
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messageCount);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _fixture.RabbitDeliveryService.PublishManyAsBatchesAsync(exchangeName, queueName, payloads, 7, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var queueCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var results = await _fixture.RabbitDeliveryService.GetAllAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(queueCount == messageCount, "Messages were lost in routing.");
            Assert.True(results.Count == messageCount);
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishManyAsBatchesInParallelAsync()
        {
            // Arrange
            var messageCount = 100;
            var queueName = $"{_fixture.TestQueueName4}.2444";
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messageCount);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            await _fixture.RabbitDeliveryService.PublishManyAsBatchesInParallelAsync(exchangeName, queueName, payloads, 10, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var queueCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var results = await _fixture.RabbitDeliveryService.GetAllAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(queueCount == messageCount, "Message were lost in routing.");
            Assert.True(results.Count == messageCount);
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }
    }
}
