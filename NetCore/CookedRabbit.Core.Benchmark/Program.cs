using BenchmarkDotNet.Running;
using System;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Benchmark
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Publish>();

            await Console.Out.WriteLineAsync("CookedRabbit benchmark has finished!");
        }
    }
}
