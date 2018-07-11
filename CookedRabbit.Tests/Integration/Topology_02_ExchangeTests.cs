using CookedRabbit.Tests.Integration.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integration
{
    [Collection("IntegrationTests")]
    public class Topology_02_ExchangeTests
    {
        private readonly IntegrationFixture _fixture;

        public Topology_02_ExchangeTests(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Rabbit Topology", "Queue")]
        public async Task Exchange_DeclareDelete()
        {
            // Arrange
            var exchangeName = $"{_fixture.TestExchangeName}.6111";

            // Act
            var createSuccess = await _fixture.RabbitTopologyService.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct.Description());
            var deleteSuccess = await _fixture.RabbitTopologyService.ExchangeDeleteAsync(exchangeName);

            // Assert
            Assert.True(createSuccess, "Exchange was not declared.");
            Assert.True(deleteSuccess, "Exchange was not deleted.");
        }
    }
}
