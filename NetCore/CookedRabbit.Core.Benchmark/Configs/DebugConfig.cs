using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;

namespace CookedRabbit.Core.Benchmark.Configs
{
    public class DebugConfig : ManualConfig
    {
        public DebugConfig()
        {
            Add(JitOptimizationsValidator.DontFailOnError);

            Add(Job.Default
                .WithCustomBuildConfiguration("Debug")
                .With(InProcessToolchain.Instance));
        }
    }
}
