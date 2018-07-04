using CookedRabbit.Library.Models;
using CookedRabbit.Library.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integrations
{
    public class Topology_01_QueueTests : IDisposable
    {
        private readonly RabbitService _rabbitService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName = "CookedRabbit.TopologyTestQueue";

        // Test Setup
        public Topology_01_QueueTests()
        {
            _seasoning = new RabbitSeasoning
            {
                RabbitHost = "localhost",
                ConnectionName = "RabbitServiceTest",
                ConnectionPoolCount = 1,
                ChannelPoolCount = 1
            };

            _rabbitService = new RabbitService(_seasoning);
            _rabbitTopologyService = new RabbitTopologyService(_seasoning);

            try
            { _rabbitTopologyService.QueueDeleteAsync(_testQueueName).GetAwaiter().GetResult(); }
            catch { }
        }

        [Fact]
        [Trait("Rabbit Topology - Queue", "Queue_DeclareDelete")]
        public void Queue_DeclareDelete()
        {
            // Arrange
            var queueName = _testQueueName;

            // Act
            var createSuccess = _rabbitTopologyService.QueueDeclareAsync(queueName).GetAwaiter().GetResult();
            var deleteSuccess = _rabbitTopologyService.QueueDeleteAsync(queueName).GetAwaiter().GetResult();

            // Assert
            Assert.True(createSuccess, "Queue was not declared.");
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }

        [Fact]
        [Trait("Rabbit Topology - Queue", "Queue_DeclarePublishDelete")]
        public async Task Queue_DeclarePublishDelete()
        {
            // Arrange
            var queueName = _testQueueName;
            string exchangeName = string.Empty; // Null throws error in RabbitMQ.

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitService.PublishAsync(exchangeName, queueName, await GetRandomByteArray(1000), false, null);
            var deleteSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueName, false, true);

            // Assert
            Assert.True(createSuccess, "Queue was not declared.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.False(deleteSuccess, "Queue was deleted with a message inside.");

            // Re-Act
            deleteSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueName, false, false);

            // Re-Assert
            Assert.True(deleteSuccess, "Queue was not deleted.");
        }

        [Fact]
        [Trait("Rabbit Topology - Queue", "Queue_DeclarePublishGetDelete")]
        public async Task Queue_DeclarePublishGetDelete()
        {
            // Arrange
            var queueName = _testQueueName;
            string exchangeName = string.Empty; // Null throws error in RabbitMQ.
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitService.PublishAsync(exchangeName, queueName, payload, false, null);
            var result = await _rabbitService.GetAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");

            // Re-Arrange
            var messageIdentical = await ByteArrayCompare(result.Body, payload);

            // Re-Act
            var deleteSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueName, false, false);

            // Re-Assert
            Assert.True(deleteSuccess, "Queue was not deleted.");
            Assert.True(messageIdentical, "Message received was not identical to published message.");
        }

        #region Dispose Section

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Cleanup
                    try
                    { _rabbitTopologyService.QueueDeleteAsync(_testQueueName).GetAwaiter().GetResult(); }
                    catch { }

                    _rabbitService.Dispose(true);
                    _rabbitTopologyService.Dispose(true);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
