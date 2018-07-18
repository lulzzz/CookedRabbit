using CookedRabbit.Core.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;

namespace CookedRabbit.Core.Tests.Integration.Benchmark
{
    [Collection("BenchmarkFixture")]
    public class Benchmark_01_Publish
    {
        private readonly BenchmarkFixture _fixture;

        public Benchmark_01_Publish(BenchmarkFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Core Benchmark", "PublishMany")]
        public async Task TestingAutoScaleAsync()
        {
            await _fixture.DeliveryService.PublishManyAsync(_fixture.ExchangeName, _fixture.QueueName, _fixture.Payloads, false, null);
        }
    }
}
