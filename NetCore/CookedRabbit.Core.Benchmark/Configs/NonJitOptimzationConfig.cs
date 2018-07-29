using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;

namespace CookedRabbit.Core.Benchmark.Configs
{
    public class NonJitOptimzationConfig : ManualConfig
    {
        public NonJitOptimzationConfig()
        {
            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS
        }
    }
}
