using CookedRabbit.Core.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Tests.Integration
{
    [Collection("IntegrationTests_Zero")]
    public class Delivery_01_PublishTests
    {
        private readonly IntegrationFixture_Zero _fixture;

        public Delivery_01_PublishTests(IntegrationFixture_Zero fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Core Delivery", "Publish")]
        public async Task PublishAsync()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName1}.1111";
            var exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var messageCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(messageCount > 0, "Message was lost in routing.");
            Assert.True(deleteSuccess);
        }

        [Fact]
        [Trait("Core Delivery", "Publish")]
        public async Task PublishManyAsync()
        {
            // Arrange
            var messagesToSend = 17;
            var queueName = $"{_fixture.TestQueueName2}.1112";
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _fixture.RabbitDeliveryService.PublishManyAsync(exchangeName, queueName, payloads, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var messageCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(messageCount == messagesToSend, "Messages were lost in routing.");
            Assert.True(deleteSuccess);
        }

        [Fact]
        [Trait("Core Delivery", "Publish")]
        public async Task PublishManyAsBatchesAsync()
        {
            // Arrange
            var messagesToSend = 99;
            var queueName = $"{_fixture.TestQueueName3}.1113";
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _fixture.RabbitDeliveryService.PublishManyAsBatchesAsync(exchangeName, queueName, payloads, 15, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var messageCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(messageCount == messagesToSend, "Messages were lost in routing.");
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }

        [Fact]
        [Trait("Core Delivery", "Publish")]
        public async Task PublishManyAsBatchesInParallelAsync()
        {
            // Arrange
            var messagesToSend = 99;
            var queueName = $"{_fixture.TestQueueName4}.1114";
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            await _fixture.RabbitDeliveryService.PublishManyAsBatchesInParallelAsync(exchangeName, queueName, payloads, 10, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var messageCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(messageCount == messagesToSend, "Messages were lost in routing.");
            Assert.True(deleteSuccess);
        }
    }
}
