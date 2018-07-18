using CookedRabbit.Core.Tests.Fixtures;
using Xunit;

namespace CookedRabbit.Core.Tests.Integration
{
    [Collection("IntegrationTests_Zero")]
    public class Serialize_02_ZeroFormat_PublishGetTests
    {
        private readonly IntegrationFixture_Zero _fixture;

        public Serialize_02_ZeroFormat_PublishGetTests(IntegrationFixture_Zero fixture)
        {
            _fixture = fixture;
        }
    }
}
