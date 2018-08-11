﻿using CookedRabbit.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Tests.Integration
{
    [Collection("IntegrationTests_NoCompression_Utf8")]
    public class Topology_02_ExchangeTests
    {
        private readonly ZeroFormat_NoCompression _fixture;

        public Topology_02_ExchangeTests(ZeroFormat_NoCompression fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Net Topology", "Queue")]
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
