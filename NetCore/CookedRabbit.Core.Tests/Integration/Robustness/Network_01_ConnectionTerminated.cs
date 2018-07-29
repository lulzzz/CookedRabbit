using CookedRabbit.Core.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Tests.Integration.Robustness
{
    [Collection("Standard")]
    public class Network_01_ConnectionTerminated
    {
        private readonly StandardFixture _fixture;

        public Network_01_ConnectionTerminated(StandardFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("NetCore Network", "ConnectionTerminated")]
        public async Task ConnectionTerminatedWhilePublishing()
        {
            // Arrange
            var payloads = await CreatePayloadsAsync(100, 150);
            var exchangeName = string.Empty;
            var queueName = "CookedRabbit.NetworkTest";
            await _fixture.Burrow.Maintenance.QueueDeclareAsync(queueName);
            await _fixture.Burrow.Maintenance.PurgeQueueAsync(queueName);

            // Act
            _fixture.Burrow.Transmission.CloseConnections();
            var failures = await _fixture.Burrow.Transmission.PublishManyAsync(exchangeName, queueName, payloads, false, null);

            // Assert
            Assert.NotEmpty(failures);
        }
    }
}
