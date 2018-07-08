using CookedRabbit.Library.Models;
using CookedRabbit.Library.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Tests.Integrations
{
    public class Topology_03_RoutingTests : IDisposable
    {
        private readonly RabbitDeliveryService _rabbitDeliveryService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName1 = "CookedRabbit.TopologyTestQueue1";
        private readonly string _testQueueName2 = "CookedRabbit.TopologyTestQueue2";
        private readonly string _testQueueName3 = "CookedRabbit.TopologyTestQueue3";
        private readonly string _testExchangeName = "CookedRabbit.TopologyTestExchange";

        // TODO: Convert Tests To Collections https://xunit.github.io/docs/shared-context.html#collection-fixture
        // Test Setup
        public Topology_03_RoutingTests()
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
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName1, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName2, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName3, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName, false).GetAwaiter().GetResult();
            }
            catch { }
        }

        [Fact]
        [Trait("Rabbit Topology", "Routing")]
        public async Task Exchange_DirectPublishGetDelete()
        {
            // Arrange
            var queueName = _testQueueName1;
            var routingKey = _testQueueName1;
            var exchangeName = _testExchangeName;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createQueueSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var createExchangeSuccess = await _rabbitTopologyService.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct.Description());
            var bindQueueToExchangeSuccess = await _rabbitTopologyService.QueueBindToExchangeAsync(queueName, exchangeName, routingKey);
            var publishSuccess = await _rabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);
            var messageCount = await _rabbitDeliveryService.GetMessageCountAsync(queueName);
            var result = await _rabbitDeliveryService.GetAsync(queueName);

            // Assert
            Assert.True(createQueueSuccess, "Queue was not created.");
            Assert.True(createExchangeSuccess, "Exchange was not created.");
            Assert.True(bindQueueToExchangeSuccess, "Queue was not bound to Exchange.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(messageCount > 0, "Message was lost in routing.");
            Assert.False(result is null, "Result was null.");

            // Re-Arrange
            var messageIdentical = await ByteArrayCompare(result.Body, payload);

            // Re-Act
            var deleteQueueSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueName, false, false);
            var deleteExchangeSuccess = await _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName);

            // Re-Assert
            Assert.True(deleteQueueSuccess, "Queue was not deleted.");
            Assert.True(deleteExchangeSuccess, "Exchange was not deleted.");
            Assert.True(messageIdentical, "Message received was not identical to published message.");
        }

        [Fact]
        [Trait("Rabbit Topology", "Routing")]
        public async Task Exchange_FanoutPublishGetDelete()
        {
            // Arrange
            var queueNameOne = $"{_testQueueName2}.1";
            var queueNameTwo = $"{_testQueueName2}.2";
            var queueNameThree = $"{_testQueueName2}.3";
            var routingKey = _testQueueName2;
            var exchangeName = _testExchangeName;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createQueueOneSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueNameOne);
            var createQueueTwoSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueNameTwo);
            var createQueueThreeSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueNameThree);

            var createExchangeSuccess = await _rabbitTopologyService.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout.Description());

            var bindQueueOneToExchangeSuccess = await _rabbitTopologyService.QueueBindToExchangeAsync(queueNameOne, exchangeName);
            var bindQueueTwoToExchangeSuccess = await _rabbitTopologyService.QueueBindToExchangeAsync(queueNameTwo, exchangeName);
            var bindQueueThreeToExchangeSuccess = await _rabbitTopologyService.QueueBindToExchangeAsync(queueNameThree, exchangeName);

            var publishSuccess = await _rabbitDeliveryService.PublishAsync(exchangeName, string.Empty, payload, false, null);

            var messageCountQueueOne = await _rabbitDeliveryService.GetMessageCountAsync(queueNameOne);
            var messageCountQueueTwo = await _rabbitDeliveryService.GetMessageCountAsync(queueNameTwo);
            var messageCountQueueThree = await _rabbitDeliveryService.GetMessageCountAsync(queueNameThree);

            var resultOne = await _rabbitDeliveryService.GetAsync(queueNameOne);
            var resultTwo = await _rabbitDeliveryService.GetAsync(queueNameTwo);
            var resultThree = await _rabbitDeliveryService.GetAsync(queueNameThree);

            // Assert
            Assert.True(createQueueOneSuccess, "Queue one was not created.");
            Assert.True(createQueueTwoSuccess, "Queue two was not created.");
            Assert.True(createQueueThreeSuccess, "Queue three was not created.");

            Assert.True(createExchangeSuccess, "Exchange was not created.");

            Assert.True(bindQueueOneToExchangeSuccess, "Queue one was not bound to Exchange.");
            Assert.True(bindQueueOneToExchangeSuccess, "Queue two was not bound to Exchange.");
            Assert.True(bindQueueOneToExchangeSuccess, "Queue three was not bound to Exchange.");

            Assert.True(publishSuccess, "Message failed to publish.");

            Assert.True(messageCountQueueOne > 0, "Message was lost in routing to queue one.");
            Assert.True(messageCountQueueTwo > 0, "Message was lost in routing to queue two.");
            Assert.True(messageCountQueueThree > 0, "Message was lost in routing to queue three.");

            Assert.False(resultOne is null, "Result one was null.");
            Assert.False(resultTwo is null, "Result two was null.");
            Assert.False(resultThree is null, "Result three was null.");

            // Re-Arrange
            var messageOneIdentical = await ByteArrayCompare(resultOne.Body, payload);
            var messageTwoIdentical = await ByteArrayCompare(resultTwo.Body, payload);
            var messageThreeIdentical = await ByteArrayCompare(resultThree.Body, payload);

            // Re-Act
            var deleteQueueOneSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueNameOne, false, false);
            var deleteQueueTwoSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueNameTwo, false, false);
            var deleteQueueThreeSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueNameThree, false, false);
            var deleteExchangeSuccess = await _rabbitTopologyService.ExchangeDeleteAsync(exchangeName);

            // Re-Assert
            Assert.True(deleteQueueOneSuccess, "Queue one was not deleted.");
            Assert.True(deleteQueueTwoSuccess, "Queue two was not deleted.");
            Assert.True(deleteQueueThreeSuccess, "Queue three was not deleted.");
            Assert.True(deleteExchangeSuccess, "Exchange was not deleted.");

            Assert.True(messageOneIdentical, "Message one received was not identical to published message.");
            Assert.True(messageTwoIdentical, "Message two received was not identical to published message.");
            Assert.True(messageThreeIdentical, "Message three received was not identical to published message.");
        }

        [Fact]
        [Trait("Rabbit Topology", "Routing")]
        public async Task Exchange_TopicPublishGetDelete()
        {
            // Arrange
            var queueNameOne = $"{_testQueueName3}.1";
            var queueNameTwo = $"{_testQueueName3}.2";
            var queueNameThree = $"{_testQueueName3}.3";

            var topicKeyOne = "#";
            var topicKeyTwo = "house.#";
            var topicKeyThree = "house.cat";

            var routingKey = _testQueueName3;
            var exchangeName = _testExchangeName;
            var payload = await GetRandomByteArray(1000);

            // Act
            var createQueueOneSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueNameOne);
            var createQueueTwoSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueNameTwo);
            var createQueueThreeSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueNameThree);

            var createExchangeSuccess = await _rabbitTopologyService.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic.Description());

            var bindQueueOneToExchangeSuccess = await _rabbitTopologyService.QueueBindToExchangeAsync(queueNameOne, exchangeName, topicKeyOne);
            var bindQueueTwoToExchangeSuccess = await _rabbitTopologyService.QueueBindToExchangeAsync(queueNameTwo, exchangeName, topicKeyTwo);
            var bindQueueThreeToExchangeSuccess = await _rabbitTopologyService.QueueBindToExchangeAsync(queueNameThree, exchangeName, topicKeyThree);

            var publishSuccessOne = await _rabbitDeliveryService.PublishAsync(exchangeName, topicKeyOne, payload, false, null);
            var publishSuccessTwo = await _rabbitDeliveryService.PublishAsync(exchangeName, topicKeyTwo, payload, false, null);
            var publishSuccessThree = await _rabbitDeliveryService.PublishAsync(exchangeName, topicKeyThree, payload, false, null);

            var messageCountQueueOne = await _rabbitDeliveryService.GetMessageCountAsync(queueNameOne);
            var messageCountQueueTwo = await _rabbitDeliveryService.GetMessageCountAsync(queueNameTwo);
            var messageCountQueueThree = await _rabbitDeliveryService.GetMessageCountAsync(queueNameThree);

            var resultOne = await _rabbitDeliveryService.GetAsync(queueNameOne);
            var resultTwo = await _rabbitDeliveryService.GetAsync(queueNameTwo);
            var resultThree = await _rabbitDeliveryService.GetAsync(queueNameThree);

            // Assert
            Assert.True(createQueueOneSuccess, "Queue one was not created.");
            Assert.True(createQueueTwoSuccess, "Queue two was not created.");
            Assert.True(createQueueThreeSuccess, "Queue three was not created.");

            Assert.True(createExchangeSuccess, "Exchange was not created.");

            Assert.True(bindQueueOneToExchangeSuccess, "Queue one was not bound to Exchange.");
            Assert.True(bindQueueOneToExchangeSuccess, "Queue two was not bound to Exchange.");
            Assert.True(bindQueueOneToExchangeSuccess, "Queue three was not bound to Exchange.");

            Assert.True(publishSuccessOne, "Message one failed to publish.");
            Assert.True(publishSuccessTwo, "Message two failed to publish.");
            Assert.True(publishSuccessThree, "Message three failed to publish.");

            Assert.True(messageCountQueueOne == 3, $"Message was lost in routing to queue one. Had {messageCountQueueOne} Expected {3}");
            Assert.True(messageCountQueueTwo == 2, $"Message was lost in routing to queue two. Had {messageCountQueueTwo} Expected {2}");
            Assert.True(messageCountQueueThree == 1, $"Message was lost in routing to queue three. Had {messageCountQueueThree} Expected {1}");

            Assert.False(resultOne is null, "Result one was null.");
            Assert.False(resultTwo is null, "Result two was null.");
            Assert.False(resultThree is null, "Result three was null.");

            // Re-Arrange
            var messageOneIdentical = await ByteArrayCompare(resultOne.Body, payload);
            var messageTwoIdentical = await ByteArrayCompare(resultTwo.Body, payload);
            var messageThreeIdentical = await ByteArrayCompare(resultThree.Body, payload);

            // Re-Act
            var deleteQueueOneSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueNameOne, false, false);
            var deleteQueueTwoSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueNameTwo, false, false);
            var deleteQueueThreeSuccess = await _rabbitTopologyService.QueueDeleteAsync(queueNameThree, false, false);
            var deleteExchangeSuccess = await _rabbitTopologyService.ExchangeDeleteAsync(exchangeName);

            // Re-Assert
            Assert.True(deleteQueueOneSuccess, "Queue one was not deleted.");
            Assert.True(deleteQueueTwoSuccess, "Queue two was not deleted.");
            Assert.True(deleteQueueThreeSuccess, "Queue three was not deleted.");
            Assert.True(deleteExchangeSuccess, "Exchange was not deleted.");

            Assert.True(messageOneIdentical, "Message one received was not identical to published message.");
            Assert.True(messageTwoIdentical, "Message two received was not identical to published message.");
            Assert.True(messageThreeIdentical, "Message three received was not identical to published message.");
        }

        #region Dispose Section

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName1, false, false).GetAwaiter().GetResult();
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName2, false, false).GetAwaiter().GetResult();
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName3, false, false).GetAwaiter().GetResult();
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
