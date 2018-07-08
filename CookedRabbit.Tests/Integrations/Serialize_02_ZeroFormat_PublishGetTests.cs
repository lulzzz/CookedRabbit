using AutoFixture;
using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Services;
using CookedRabbit.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Tests.Integrations
{
    public class Serialize_02_ZeroFormat_PublishGetTests : IDisposable
    {
        private readonly IRabbitChannelPool _rchanp;
        private readonly IRabbitConnectionPool _rconp;
        private readonly RabbitDeliveryService _rabbitDeliveryService;
        private readonly RabbitSerializeService _rabbitSerializeService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName1 = "CookedRabbit.SerializeTestQueue1";
        private readonly string _testQueueName2 = "CookedRabbit.SerializeTestQueue2";
        private readonly string _testQueueName3 = "CookedRabbit.SerializeTestQueue3";
        private readonly string _testExchangeName = "CookedRabbit.SerializeTestExchange";

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

            _rchanp.SetConnectionPoolAsync(_seasoning, _rconp).GetAwaiter().GetResult();

            _rabbitDeliveryService = new RabbitDeliveryService(_seasoning, _rchanp);
            _rabbitSerializeService = new RabbitSerializeService(_seasoning, _rchanp);
            _rabbitTopologyService = new RabbitTopologyService(_seasoning, _rchanp);

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
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression PublishGet")]
        public async Task PublishExceptionAsync()
        {
            // Arrange
            var queueName = _testQueueName1;
            var exchangeName = string.Empty;

            var testObject = new TestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative", "spraytan", "traitor", "peetape", "kompromat" }
            };

            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            Assert.True(createSuccess, "Queue was not created.");
            await Assert.ThrowsAsync<System.InvalidOperationException>(() => _rabbitSerializeService.SerializeAndPublishAsync(testObject, envelope));
        }

        [Fact]
        [Trait("Rabbit Serialize", "ZeroFormat NoCompression PublishGet")]
        public async Task PublishAndGetAsync()
        {
            // Arrange
            var queueName = _testQueueName2;
            var exchangeName = string.Empty;

            var testObject = new ZeroTestHelperObject
            {
                Name = "RussianTestObject",
                Address = "1600 Pennsylvania Ave.",
                BitsAndPieces = new List<string> { "russianoperative" , "spraytan", "traitor", "peetape", "kompromat" }
            };

            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var publishSuccess = await _rabbitSerializeService.SerializeAndPublishAsync(testObject, envelope);
            var result = await _rabbitSerializeService.GetAndDeserializeAsync<ZeroTestHelperObject>(queueName);

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

            // Arrange
            var messageCount = 17;
            var messages = fixture.CreateMany<ZeroTestHelperObject>(messageCount).ToList();
            var queueName = _testQueueName3;
            var exchangeName = string.Empty;
            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                ContentEncoding = ContentEncoding.Binary,
                MessageType = $"{ContentType.Textplain.Description()}{Charset.Utf8.Description()}"
            };

            // Act
            var createSuccess = await _rabbitTopologyService.QueueDeclareAsync(queueName);
            var failures = await _rabbitSerializeService.SerializeAndPublishManyAsync(messages, envelope);
            var results = await _rabbitSerializeService.GetAndDeserializeManyAsync<ZeroTestHelperObject>(queueName, messageCount);

            // Assert
            Assert.True(createSuccess, "Queue was not created.");
            Assert.Empty(failures);
            Assert.True(results.Count == messageCount, "Messages were lost.");
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
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName1).GetAwaiter().GetResult();
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName2).GetAwaiter().GetResult();
                        _rabbitTopologyService.QueueDeleteAsync(_testQueueName3).GetAwaiter().GetResult();
                    }
                    catch { }

                    _rabbitDeliveryService.Dispose(true);
                    _rabbitSerializeService.Dispose(true);
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
