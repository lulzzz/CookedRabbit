using CookedRabbit.Tests.Integration.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integration
{
    [Collection("IntegrationTests")]
    public class Topology_01_QueueTests
    {
        private readonly IntegrationFixture _fixture;

        public Topology_01_QueueTests(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Rabbit Topology", "Queue")]
        public async Task Queue_DeclareDelete()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName1}.5111";

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not declared.");
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }

        [Fact]
        [Trait("Rabbit Topology", "Queue")]
        public async Task Queue_DeclarePublishDelete()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName2}.5112";
            string exchangeName = string.Empty;

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, queueName, await GetRandomByteArray(1000), false, null);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName, false, true);

            // Assert
            Assert.True(createSuccess, "Queue was not declared.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.False(deleteSuccess, "Queue was deleted with a message inside.");
        }

        [Fact]
        [Trait("Rabbit Topology", "Queue")]
        public async Task Queue_DeclarePublishGetDelete()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName3}.5113";
            string exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);
            var result = await _fixture.RabbitDeliveryService.GetAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");

            // Re-Arrange
            var messageIdentical = await ByteArrayCompare(result.Body, payload);

            // Re-Act
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName, false, false);

            // Re-Assert
            Assert.True(deleteSuccess, "Queue was not deleted.");
            Assert.True(messageIdentical, "Message received was not identical to published message.");
        }
    }
}
