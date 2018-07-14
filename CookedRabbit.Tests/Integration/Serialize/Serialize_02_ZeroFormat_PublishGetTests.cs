using CookedRabbit.Tests.Fixtures;
using Xunit;

namespace CookedRabbit.Tests.Integration
{
    [Collection("IntegrationTests")]
    public class Serialize_02_ZeroFormat_PublishGetTests
    {
        private readonly IntegrationFixture _fixture;

        public Serialize_02_ZeroFormat_PublishGetTests(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }
    }
}
