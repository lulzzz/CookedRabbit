using CookedRabbit.Core.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Tests.Integration
{
    [Collection("IntegrationTests_Zero")]
    public class Scaling_01_ChannelPool
    {
        private readonly IntegrationFixture_Zero _fixture;

        public Scaling_01_ChannelPool(IntegrationFixture_Zero fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Core Scaling", "AutoScaling ChannelPool")]
        public async Task TestingAutoScaleAsync()
        {
            // Arrange
            var messagesToSend = 10000;
            var queueName1 = $"{_fixture.TestQueueName1}.Scaling1";
            var queueName2 = $"{_fixture.TestQueueName1}.Scaling2";
            var queueName3 = $"{_fixture.TestQueueName1}.Scaling3";
            var queueName4 = $"{_fixture.TestQueueName1}.Scaling4";

            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend, 100);

            // Act
            var createSuccess1 = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName1);
            var createSuccess2 = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName2);
            var createSuccess3 = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName3);
            var createSuccess4 = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName4);

            var task1 = _fixture.RabbitDeliveryService.PublishManyAsync(exchangeName, queueName1, payloads, false, null);
            var task2 = _fixture.RabbitDeliveryService.PublishManyAsync(exchangeName, queueName2, payloads, false, null);
            var task3 = _fixture.RabbitDeliveryService.PublishManyAsync(exchangeName, queueName3, payloads, false, null);
            var task4 = _fixture.RabbitDeliveryService.PublishManyAsync(exchangeName, queueName4, payloads, false, null);

            await Task.WhenAll(new Task[] { task1, task2, task3, task4 });
            await Task.Delay(500); // Message count (server side) requires a delay for accuracy

            var messageCount1 = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName1);
            var messageCount2 = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName2);
            var messageCount3 = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName3);
            var messageCount4 = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName4);

            var deleteSuccess1 = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName1);
            var deleteSuccess2 = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName2);
            var deleteSuccess3 = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName3);
            var deleteSuccess4 = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName4);

            var autoScaleCount = _fixture.RabbitDeliveryService.GetAutoScalingIterationCount();

            // Assert
            Assert.True(createSuccess1, "Queue was not created.");
            Assert.True(createSuccess2, "Queue was not created.");
            Assert.True(createSuccess3, "Queue was not created.");
            Assert.True(createSuccess4, "Queue was not created.");

            Assert.True(messageCount1 == messagesToSend, "Messages were lost in routing.");
            Assert.True(messageCount2 == messagesToSend, "Messages were lost in routing.");
            Assert.True(messageCount3 == messagesToSend, "Messages were lost in routing.");
            Assert.True(messageCount4 == messagesToSend, "Messages were lost in routing.");

            Assert.True(deleteSuccess1);
            Assert.True(deleteSuccess2);
            Assert.True(deleteSuccess3);
            Assert.True(deleteSuccess4);
        }
    }
}
