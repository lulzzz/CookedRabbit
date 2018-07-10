using CookedRabbit.Library.Models;
using CookedRabbit.Library.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integration
{
    public class Delivery_01_PublishTests : IDisposable
    {
        private readonly RabbitDeliveryService _rabbitDeliveryService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName1 = "CookedRabbit.DeliveryTestQueue1";
        private readonly string _testQueueName2 = "CookedRabbit.DeliveryTestQueue2";
        private readonly string _testQueueName3 = "CookedRabbit.DeliveryTestQueue3";
        private readonly string _testQueueName4 = "CookedRabbit.DeliveryTestQueue4";
        private readonly string _testExchangeName = "CookedRabbit.DeliveryServiceTestExchange";

        public Delivery_01_PublishTests()
        {
            _seasoning = new RabbitSeasoning
            {
                RabbitHostName = "localhost",
                ConnectionName = "RabbitServiceTest",
                ConnectionPoolCount = 1,
                ChannelPoolCount = 1,
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false
            };

            _rabbitDeliveryService = new RabbitDeliveryService(_seasoning);
            _rabbitTopologyService = new RabbitTopologyService(_seasoning);

            try
            {
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName1, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName2, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName3, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName4, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName, false).GetAwaiter().GetResult();
            }
            catch { }
        }

        [Fact]
        [Trait("Rabbit Delivery", "Publish")]
        public async Task PublishAsync()
        {
            // Arrange
            var queueName = _testQueueName1;
            var exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);
            var messageCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(messageCount > 0, "Message was lost in routing.");
        }

        [Fact]
        [Trait("Rabbit Delivery", "Publish")]
        public async Task PublishManyAsync()
        {
            // Arrange
            var messagesToSend = 17;
            var queueName = _testQueueName2;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitDeliveryService.PublishManyAsync(exchangeName, queueName, payloads, false, null);
            var messageCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(messageCount == messagesToSend, "Messages were lost in routing.");
        }

        [Fact]
        [Trait("Rabbit Delivery", "Publish")]
        public async Task PublishManyAsBatchesAsync()
        {
            // Arrange
            var messagesToSend = 111;
            var queueName = _testQueueName3;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitDeliveryService.PublishManyAsBatchesAsync(exchangeName, queueName, payloads, 7, false, null);
            var messageCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(messageCount == messagesToSend, "Messages were lost in routing.");
        }

        [Fact]
        [Trait("Rabbit Delivery", "Publish")]
        public async Task PublishManyAsBatchesInParallelAsync()
        {
            // Arrange
            var messagesToSend = 100;
            var queueName = _testQueueName4;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            await _rabbitDeliveryService.PublishManyAsBatchesInParallelAsync(exchangeName, queueName, payloads, 10, false, null);
            var messageCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(messageCount == messagesToSend, "Message were lost in routing.");
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
                    {
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName1, false, false).GetAwaiter().GetResult();
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName2, false, false).GetAwaiter().GetResult();
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName3, false, false).GetAwaiter().GetResult();
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName4, false, false).GetAwaiter().GetResult();
                        _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName, false).GetAwaiter().GetResult();
                    }
                    catch { }

                    _rabbitDeliveryService.Dispose(true);
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
