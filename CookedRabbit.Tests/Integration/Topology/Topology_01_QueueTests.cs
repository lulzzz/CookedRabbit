using CookedRabbit.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integration
{
    [Collection("IntegrationTests_NoCompression_ZeroFormat")]
    public class Topology_01_QueueTests
    {
        private readonly ZeroFormat_NoCompression _fixture;

        public Topology_01_QueueTests(ZeroFormat_NoCompression fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Net Topology", "Queue")]
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
        [Trait("Net Topology", "Queue")]
        public async Task Queue_DeclarePublishDelete()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName2}.5112";
            string exchangeName = string.Empty;

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, queueName, await GetRandomByteArray(1000), false, null);

            await Task.Delay(100);

            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName, false, true);

            // Assert
            Assert.True(createSuccess, "Queue was not declared.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.False(deleteSuccess, "Queue was deleted with a message inside.");

            // Re-Act
            deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName, false, false);
            Assert.True(deleteSuccess, "Queue failed to delete.");
        }

        [Fact]
        [Trait("Net Topology", "Queue")]
        public async Task Queue_DeclarePublishGetDelete()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName3}.5113";
            string exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);

            await Task.Delay(100);

            var result = await _fixture.RabbitDeliveryService.GetAsync(queueName);
            var messageIdentical = await ByteArrayCompare(result.Body, payload);
            var deleteSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName, false, false);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");
            Assert.True(deleteSuccess, "Queue was not deleted.");
            Assert.True(messageIdentical, "Message received was not identical to published message.");
        }
    }
}
