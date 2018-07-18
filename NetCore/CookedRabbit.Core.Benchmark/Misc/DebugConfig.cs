using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using System.Linq;

namespace CookedRabbit.Core.Benchmark.Misc
{
    public class DebugConfig : ManualConfig
    {
        public DebugConfig()
        {
            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS
        }
    }
}
