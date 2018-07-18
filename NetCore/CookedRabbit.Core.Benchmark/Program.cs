using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CookedRabbit.Core.Benchmark.Misc;
using System;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Benchmark
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Publish>();
            await Console.In.ReadLineAsync();
        }
    }
}
