using CookedRabbit.Library.Models;
using CookedRabbit.Library.Services;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integrations
{
    public class Delivery_02_PublishGetTests
    {
        private readonly RabbitDeliveryService _rabbitDeliveryService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName = "CookedRabbit.RabbitServiceTestQueue";
        private readonly string _testExchangeName = "CookedRabbit.RabbitServiceTestExchange";

        public Delivery_02_PublishGetTests()
        {
            _seasoning = new RabbitSeasoning
            {
                RabbitHost = "localhost",
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
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName, false).GetAwaiter().GetResult();
            }
            catch { }
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishAndGetAsync()
        {
            // Arrange
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);
            var result = await _rabbitDeliveryService.GetAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishManyAndGetManyAsync()
        {
            // Arrange
            var messageCount = 17;
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messageCount);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitDeliveryService.PublishManyAsync(exchangeName, queueName, payloads, false, null);
            var queueCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(queueCount == messageCount, "Messages were lost in routing.");

            // Re-Act
            var results = await _rabbitDeliveryService.GetManyAsync(queueName, messageCount);

            // Re-Assert
            Assert.True(results.Count == messageCount);
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishManyAsBatchesAsync()
        {
            // Arrange
            var messageCount = 111;
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messageCount);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitDeliveryService.PublishManyAsBatchesAsync(exchangeName, queueName, payloads, 7, false, null);
            var queueCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(queueCount == messageCount, "Messages were lost in routing.");

            // Re-Act
            var results = await _rabbitDeliveryService.GetAllAsync(queueName);

            // Re-Assert
            Assert.True(results.Count == messageCount);
        }

        [Fact]
        [Trait("Rabbit Delivery", "PublishGet")]
        public async Task PublishManyAsBatchesInParallelAsync()
        {
            // Arrange
            var messageCount = 100;
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messageCount);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            await _rabbitDeliveryService.PublishManyAsBatchesInParallelAsync(exchangeName, queueName, payloads, 10, false, null);
            var queueCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(queueCount == messageCount, "Message were lost in routing.");

            // Re-Act
            var results = await _rabbitDeliveryService.GetAllAsync(queueName);

            // Re-Assert
            Assert.True(results.Count == messageCount);
        }
    }
}
