using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Benchmark.Misc
{
    public class Payloads
    {
        [Params(1, 10, 100, 1000)]
        public int MessagesToCreate { get; set; }
        [Params(100, 200, 500, 1000)]
        public int MessageSizes { get; set; }

        [Benchmark]
        public async Task PayloadCreation()
        {
            var payloads = await CreatePayloadsAsync(MessagesToCreate, MessageSizes);
        }
    }
}
