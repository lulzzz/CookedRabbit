using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Tests.Fixtures;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Tests.Integration.Api
{
    [Collection("Standard")]
    public class Api_01_Gets
    {
        private readonly StandardFixture _fixture;

        public Api_01_Gets(StandardFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("NetCore Api", "GetConnections")]
        public async Task GetConnectionsFromApiAsync()
        {
            // Arrange
            await Task.Delay(5000);

            // Act
            var connections = await _fixture.Burrow.Maintenance.Api_GetAsync<List<Connection>>(RabbitApiTarget.Connections);

            // Assert
            Assert.NotEmpty(connections);
        }

        [Fact]
        [Trait("NetCore Api", "GetChannels")]
        public async Task GetChannelsFromApiAsync()
        {
            // Arrange
            await Task.Delay(5000);

            // Act
            var connections = await _fixture.Burrow.Maintenance.Api_GetAsync<List<Channel>>(RabbitApiTarget.Channels);

            // Assert
            Assert.NotEmpty(connections);
        }
    }
}
