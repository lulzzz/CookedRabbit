using BenchmarkDotNet.Attributes;
using System.Threading;

namespace CookedRabbit.Core.Benchmark.Tests
{
    public class ThreadSleep
    {
        [Benchmark]
        public void Time50() => Thread.Sleep(50);

        [Benchmark(Baseline = true)]
        public void Time100() => Thread.Sleep(100);
    }
}
