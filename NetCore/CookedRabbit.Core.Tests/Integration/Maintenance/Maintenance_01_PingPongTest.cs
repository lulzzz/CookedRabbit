using CookedRabbit.Core.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;

namespace CookedRabbit.Core.Tests.Integration.Maintenance
{
    [Collection("Standard")]
    public class Maintenance_01_PingPongTest
    {
        private readonly StandardFixture _fixture;

        public Maintenance_01_PingPongTest(StandardFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("NetCore Maintenance", "PingPong")]
        public async Task VerifyPingPongs()
        {
            // Arrange
            await _fixture.Burrow.Maintenance.PurgeQueueAsync("CookedRabbit.Maintenance.PingPong");
            await Task.Delay(15000);

            // Act
            var (Misses, AverageResponseTime) = _fixture.Burrow.Maintenance.GetAverageResponseTimes();
            var responseTimesGood = AverageResponseTime > 0 && AverageResponseTime <= 100;

            // Assert
            Assert.Equal(0, Misses);
            Assert.True(responseTimesGood, "Average response time was too high.");
        }
    }
}
