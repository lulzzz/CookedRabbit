using CookedRabbit.Library.Models;
using CookedRabbit.Tests.Fixtures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Tests.Integration.Api
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
        [Trait("Net Api", "GetConnections")]
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
        [Trait("Net Api", "GetChannels")]
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
