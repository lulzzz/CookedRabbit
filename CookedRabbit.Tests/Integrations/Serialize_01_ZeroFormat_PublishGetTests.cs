using AutoFixture;
using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Tests.Integrations
{
    public class Serialize_02_ZeroFormat_PublishGetTests
    {
        private readonly IRabbitChannelPool _rchanp;
        private readonly IRabbitConnectionPool _rconp;
        private readonly RabbitDeliveryService _rabbitDeliveryService;
        private readonly RabbitSerializeService _rabbitSerializeService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName = "CookedRabbit.RabbitSerializeServiceTestQueue";
        private readonly string _testExchangeName = "CookedRabbit.RabbitSerializeServiceTestExchange";

        public Serialize_02_ZeroFormat_PublishGetTests()
        {
            _seasoning = new RabbitSeasoning
            {
                RabbitHost = "localhost",
                ConnectionName = "RabbitSerializeServiceTest",
                ConnectionPoolCount = 1,
                ChannelPoolCount = 1,
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false,
                SerializationMethod = SerializationMethod.ZeroFormat,
                CompressionEnabled = false,
                CompressionMethod = CompressionMethod.LZ4
            };

            _rchanp = new RabbitChannelPool();
            _rconp = new RabbitConnectionPool();

            _rchanp.SetConnectionPoolAsync(_seasoning, _rconp);

            _rabbitDeliveryService = new RabbitDeliveryService(_seasoning, _rchanp);
            _rabbitSerializeService = new RabbitSerializeService(_seasoning, _rchanp);
            _rabbitTopologyService = new RabbitTopologyService(_seasoning, _rchanp);

            try
            {
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName, false).GetAwaiter().GetResult();
            }
            catch { }
        }

        [Fact]
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression PublishGet")]
        public async Task PublishAndGetAsync()
        {
            // Arrange
            var queueName = _testQueueName;
            var exchangeName = string.Empty;

            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative" , "spraytan", "traitor", "peetape" }
            };

            var envelope = new Envelope
            {
                ExchangeName = _testExchangeName,
                RoutingKey = _testQueueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitSerializeService.SerializeAndPublishAsync(testObject, envelope);
            var result = await _rabbitSerializeService.GetAndDeserializeAsync<TestHelperObject>(queueName);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.True(publishSuccess, "Message failed to publish.");
            Assert.True(result != null, "Result was null.");
        }

        [Fact]
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression PublishGet")]
        public async Task PublishManyAndGetManyAsync()
        {
            var fixture = new Fixture();
            var messages = fixture.Create<List<TestHelperObject>>();

            // Arrange
            var messageCount = 17;
            var queueName = _testQueueName;
            var exchangeName = string.Empty;
            var envelope = new Envelope
            {
                ExchangeName = _testExchangeName,
                RoutingKey = _testQueueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitSerializeService.SerializeAndPublishManyAsync(messages, envelope);
            var results = await _rabbitSerializeService.GetAndDeserializeManyAsync<TestHelperObject>(queueName, messageCount);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(results.Count == messageCount);
        }
    }
}
