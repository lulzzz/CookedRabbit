using CookedRabbit.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integration
{
    [Collection("IntegrationTests_NoCompression_ZeroFormat")]
    public class Topology_03_RoutingTests
    {
        private readonly ZeroFormat_NoCompression _fixture;

        public Topology_03_RoutingTests(ZeroFormat_NoCompression fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Net Topology", "Routing")]
        public async Task Exchange_DirectPublishGetDelete()
        {
            // Arrange
            var queueName = $"{_fixture.TestQueueName1}.7111";
            var routingKey = $"{_fixture.TestQueueName1}.7111";
            var exchangeName = $"{_fixture.TestExchangeName}.7111";
            var payload = await GetRandomByteArray(1000);

            // Act
            var createQueueSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueName);
            var createExchangeSuccess = await _fixture.RabbitTopologyService.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct.Description());
            var bindQueueToExchangeSuccess = await _fixture.RabbitTopologyService.QueueBindToExchangeAsync(queueName, exchangeName, routingKey);
            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, queueName, payload, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var messageCount = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueName);
            var result = await _fixture.RabbitDeliveryService.GetAsync(queueName);
            var deleteQueueSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueName, false, false);
            var deleteExchangeSuccess = await _fixture.RabbitTopologyService.ExchangeDeleteAsync(_fixture.TestExchangeName);
            var messageIdentical = await ByteArrayCompare(result.Body, payload);

            // Assert
            Assert.True(createQueueSuccess, "Queue was not created.");
            Assert.True(createExchangeSuccess, "Exchange was not created.");
            Assert.True(bindQueueToExchangeSuccess, "Queue was not bound to Exchange.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(messageCount > 0, "Message was lost in routing.");
            Assert.False(result is null, "Result was null.");
            Assert.True(deleteQueueSuccess, "Queue was not deleted.");
            Assert.True(deleteExchangeSuccess, "Exchange was not deleted.");
            Assert.True(messageIdentical, "Message received was not identical to published message.");
        }

        [Fact]
        [Trait("Net Topology", "Routing")]
        public async Task Exchange_FanoutPublishGetDelete()
        {
            // Arrange
            var queueNameOne = $"{_fixture.TestQueueName2}.7112";
            var queueNameTwo = $"{_fixture.TestQueueName2}.7113";
            var queueNameThree = $"{_fixture.TestQueueName2}.7114";
            var exchangeName = $"{_fixture.TestExchangeName}.7115";
            var payload = await GetRandomByteArray(1000);

            // Act
            var createQueueOneSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueNameOne);
            var createQueueTwoSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueNameTwo);
            var createQueueThreeSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueNameThree);

            var createExchangeSuccess = await _fixture.RabbitTopologyService.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout.Description());

            var bindQueueOneToExchangeSuccess = await _fixture.RabbitTopologyService.QueueBindToExchangeAsync(queueNameOne, exchangeName);
            var bindQueueTwoToExchangeSuccess = await _fixture.RabbitTopologyService.QueueBindToExchangeAsync(queueNameTwo, exchangeName);
            var bindQueueThreeToExchangeSuccess = await _fixture.RabbitTopologyService.QueueBindToExchangeAsync(queueNameThree, exchangeName);

            var publishSuccess = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, string.Empty, payload, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var messageCountQueueOne = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueNameOne);
            var messageCountQueueTwo = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueNameTwo);
            var messageCountQueueThree = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueNameThree);

            var resultOne = await _fixture.RabbitDeliveryService.GetAsync(queueNameOne);
            var resultTwo = await _fixture.RabbitDeliveryService.GetAsync(queueNameTwo);
            var resultThree = await _fixture.RabbitDeliveryService.GetAsync(queueNameThree);

            var deleteQueueOneSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueNameOne, false, false);
            var deleteQueueTwoSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueNameTwo, false, false);
            var deleteQueueThreeSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueNameThree, false, false);
            var deleteExchangeSuccess = await _fixture.RabbitTopologyService.ExchangeDeleteAsync(exchangeName);

            var messageOneIdentical = await ByteArrayCompare(resultOne.Body, payload);
            var messageTwoIdentical = await ByteArrayCompare(resultTwo.Body, payload);
            var messageThreeIdentical = await ByteArrayCompare(resultThree.Body, payload);

            // Assert
            Assert.True(createQueueOneSuccess, "Queue one was not created.");
            Assert.True(createQueueTwoSuccess, "Queue two was not created.");
            Assert.True(createQueueThreeSuccess, "Queue three was not created.");

            Assert.True(createExchangeSuccess, "Exchange was not created.");

            Assert.True(bindQueueOneToExchangeSuccess, "Queue one was not bound to Exchange.");
            Assert.True(bindQueueOneToExchangeSuccess, "Queue two was not bound to Exchange.");
            Assert.True(bindQueueOneToExchangeSuccess, "Queue three was not bound to Exchange.");

            Assert.True(publishSuccess, "Message failed to publish.");

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            Assert.True(messageCountQueueOne > 0, "Message was lost in routing to queue one.");
            Assert.True(messageCountQueueTwo > 0, "Message was lost in routing to queue two.");
            Assert.True(messageCountQueueThree > 0, "Message was lost in routing to queue three.");

            Assert.False(resultOne is null, "Result one was null.");
            Assert.False(resultTwo is null, "Result two was null.");
            Assert.False(resultThree is null, "Result three was null.");

            Assert.True(deleteQueueOneSuccess, "Queue one was not deleted.");
            Assert.True(deleteQueueTwoSuccess, "Queue two was not deleted.");
            Assert.True(deleteQueueThreeSuccess, "Queue three was not deleted.");
            Assert.True(deleteExchangeSuccess, "Exchange was not deleted.");

            Assert.True(messageOneIdentical, "Message one received was not identical to published message.");
            Assert.True(messageTwoIdentical, "Message two received was not identical to published message.");
            Assert.True(messageThreeIdentical, "Message three received was not identical to published message.");
        }

        [Fact]
        [Trait("Net Topology", "Routing")]
        public async Task Exchange_TopicPublishGetDelete()
        {
            // Arrange
            var queueNameOne = $"{_fixture.TestQueueName3}.7211";
            var queueNameTwo = $"{_fixture.TestQueueName3}.7212";
            var queueNameThree = $"{_fixture.TestQueueName3}.7213";

            var topicKeyOne = "#";
            var topicKeyTwo = "house.#";
            var topicKeyThree = "house.cat";

            var exchangeName = $"{_fixture.TestExchangeName}.7214";
            var payload = await GetRandomByteArray(1000);

            // Act
            var createQueueOneSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueNameOne);
            var createQueueTwoSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueNameTwo);
            var createQueueThreeSuccess = await _fixture.RabbitTopologyService.QueueDeclareAsync(queueNameThree);

            var createExchangeSuccess = await _fixture.RabbitTopologyService.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic.Description());

            var bindQueueOneToExchangeSuccess = await _fixture.RabbitTopologyService.QueueBindToExchangeAsync(queueNameOne, exchangeName, topicKeyOne);
            var bindQueueTwoToExchangeSuccess = await _fixture.RabbitTopologyService.QueueBindToExchangeAsync(queueNameTwo, exchangeName, topicKeyTwo);
            var bindQueueThreeToExchangeSuccess = await _fixture.RabbitTopologyService.QueueBindToExchangeAsync(queueNameThree, exchangeName, topicKeyThree);

            var publishSuccessOne = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, topicKeyOne, payload, false, null);
            var publishSuccessTwo = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, topicKeyTwo, payload, false, null);
            var publishSuccessThree = await _fixture.RabbitDeliveryService.PublishAsync(exchangeName, topicKeyThree, payload, false, null);

            await Task.Delay(200); // Message count (server side) requires a delay for accuracy

            var messageCountQueueOne = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueNameOne);
            var messageCountQueueTwo = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueNameTwo);
            var messageCountQueueThree = await _fixture.RabbitDeliveryService.GetMessageCountAsync(queueNameThree);

            var resultOne = await _fixture.RabbitDeliveryService.GetAsync(queueNameOne);
            var resultTwo = await _fixture.RabbitDeliveryService.GetAsync(queueNameTwo);
            var resultThree = await _fixture.RabbitDeliveryService.GetAsync(queueNameThree);

            var deleteQueueOneSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueNameOne, false, false);
            var deleteQueueTwoSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueNameTwo, false, false);
            var deleteQueueThreeSuccess = await _fixture.RabbitTopologyService.QueueDeleteAsync(queueNameThree, false, false);
            var deleteExchangeSuccess = await _fixture.RabbitTopologyService.ExchangeDeleteAsync(exchangeName);

            var messageOneIdentical = await ByteArrayCompare(resultOne.Body, payload);
            var messageTwoIdentical = await ByteArrayCompare(resultTwo.Body, payload);
            var messageThreeIdentical = await ByteArrayCompare(resultThree.Body, payload);

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

            Assert.True(deleteQueueOneSuccess, "Queue one was not deleted.");
            Assert.True(deleteQueueTwoSuccess, "Queue two was not deleted.");
            Assert.True(deleteQueueThreeSuccess, "Queue three was not deleted.");
            Assert.True(deleteExchangeSuccess, "Exchange was not deleted.");

            Assert.True(messageOneIdentical, "Message one received was not identical to published message.");
            Assert.True(messageTwoIdentical, "Message two received was not identical to published message.");
            Assert.True(messageThreeIdentical, "Message three received was not identical to published message.");
        }
    }
}
