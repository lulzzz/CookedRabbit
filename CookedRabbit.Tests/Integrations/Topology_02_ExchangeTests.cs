using CookedRabbit.Library.Models;
using CookedRabbit.Library.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Tests.Integrations
{
    public class Topology_02_ExchangeTests : IDisposable
    {
        private readonly RabbitDeliveryService _rabbitDeliveryService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testExchangeName = "CookedRabbit.TopologyTestExchange";

        // Test Setup
        public Topology_02_ExchangeTests()
        {
            _seasoning = new RabbitSeasoning
            {
                RabbitHost = "localhost",
                ConnectionName = "RabbitServiceTest",
                ConnectionPoolCount = 1,
                ChannelPoolCount = 1
            };

            _rabbitDeliveryService = new RabbitDeliveryService(_seasoning);
            _rabbitTopologyService = new RabbitTopologyService(_seasoning);

            try
            { _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName).GetAwaiter().GetResult(); }
            catch { }
        }

        [Fact]
        [Trait("Rabbit Topology", "Queue")]
        public async Task Exchange_DeclareDelete()
        {
            // Arrange
            var exchangeName = _testExchangeName;

            // Act
            var createSuccess = await _rabbitTopologyService.ExchangeDeclareAsync(_testExchangeName, ExchangeType.Direct.Description());
            var deleteSuccess = await _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName);

            // Assert
            Assert.True(createSuccess, "Exchange was not declared.");
            Assert.True(deleteSuccess, "Exchange was not deleted.");
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
                    { _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName).GetAwaiter().GetResult(); }
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
