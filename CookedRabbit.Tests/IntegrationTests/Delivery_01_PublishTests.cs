using CookedRabbit.Library.Models;
using CookedRabbit.Library.Services;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.UnitTests
{
    public class Delivery_01_PublishTests
    {
        private readonly RabbitDeliveryService _rabbitService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName = "CookedRabbit.RabbitServiceTestQueue";
        private readonly string _testExchangeName = "CookedRabbit.RabbitServiceTestExchange";

        public Delivery_01_PublishTests()
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

            _rabbitService = new RabbitDeliveryService(_seasoning);
            _rabbitTopologyService = new RabbitTopologyService(_seasoning);

            try
            {
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName, false).GetAwaiter().GetResult();
            }
            catch { }
        }

        [Fact]
        [Trait("Rabbit Delivery - Publish", "PublishAsync")]
        public async Task PublishAsync()
        {
            // Arrange
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitService.PublishAsync(exchangeName, queueName, payload, false, null);
            var messageCount = await _rabbitService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(messageCount > 0, "Message was lost in routing.");
        }

        [Fact]
        [Trait("Rabbit Delivery - Publish", "PublishAndGetAsync")]
        public async Task PublishAndGetAsync()
        {
            // Arrange
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitService.PublishAsync(exchangeName, queueName, payload, false, null);
            var result = await _rabbitService.GetAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");
        }

        [Fact]
        [Trait("Rabbit Delivery - Publish", "PublishManyAsync")]
        public async Task PublishManyAsync()
        {
            // Arrange
            var messagesToSend = 17;
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitService.PublishManyAsync(exchangeName, queueName, payloads, false, null);
            var messageCount = await _rabbitService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(messageCount == messagesToSend, "Messages were lost in routing.");
        }

        [Fact]
        [Trait("Rabbit Delivery - Publish", "PublishManyAsBatchesAsync")]
        public async Task PublishManyAsBatchesAsync()
        {
            // Arrange
            var messagesToSend = 111;
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitService.PublishManyAsBatchesAsync(exchangeName, queueName, payloads, 7, false, null);
            var messageCount = await _rabbitService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(messageCount == messagesToSend, "Messages were lost in routing.");
        }

        [Fact]
        [Trait("Rabbit Delivery - Publish", "PublishManyAsBatchesInParallelAsync")]
        public async Task PublishManyAsBatchesInParallelAsync()
        {
            // Arrange
            var messagesToSend = 100;
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var payloads = await CreatePayloadsAsync(messagesToSend);

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            await _rabbitService.PublishManyAsBatchesInParallelAsync(exchangeName, queueName, payloads, 10, false, null);
            var messageCount = await _rabbitService.GetMessageCountAsync(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(messageCount == messagesToSend, "Message were lost in routing.");
        }
    }
}
