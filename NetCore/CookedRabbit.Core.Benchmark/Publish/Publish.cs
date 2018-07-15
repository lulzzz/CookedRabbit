using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;
using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Benchmark
{
    [MarkdownExporter]
    [SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 2, targetCount: 3)]
    public class Publish
    {
        public Publish()
        { }

        RabbitTopologyService _topologyService;
        RabbitDeliveryService _deliveryService;
        RabbitMaintenanceService _maintenanceService;

        public List<byte[]> Payloads { get; set; }

        public string QueueName { get; set; }
        public string ExchangeName { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            QueueName = "CookedRabbit.Benchmark.Scaling";
            ExchangeName = string.Empty;

            // Configured for performance.
            var seasoning = new RabbitSeasoning
            {
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false,
                WriteErrorsToConsole = false,
                WriteErrorsToILogger = false,
                BatchBreakOnException = true
            };

            seasoning.PoolSettings.EnableAutoScaling = true;
            seasoning.PoolSettings.EmptyPoolWaitTime = 10;
            seasoning.PoolSettings.WriteSleepNoticeToConsole = false;
            seasoning.PoolSettings.ConnectionPoolCount = 4;
            seasoning.PoolSettings.ChannelPoolCount = 16;

            var channelPool = new RabbitChannelPool();
            channelPool.Initialize(seasoning).GetAwaiter().GetResult();

            _deliveryService = new RabbitDeliveryService(seasoning, channelPool);
            _topologyService = new RabbitTopologyService(seasoning, channelPool);
            _maintenanceService = new RabbitMaintenanceService(seasoning, channelPool);

            _topologyService.QueueDeclareAsync(QueueName).GetAwaiter().GetResult();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _maintenanceService.PurgeQueueAsync(QueueName).GetAwaiter().GetResult();
        }

        [Params(1, 10, 100, 1000)]
        public int MessagesToSend { get; set; }
        [Params(100, 200, 500, 1000)]
        public int MessageSize;

        [IterationSetup]
        public void IterationSetup()
        {
            _maintenanceService.PurgeQueueAsync(QueueName).GetAwaiter().GetResult();
            Payloads = CreatePayloadsAsync(MessagesToSend, MessageSize).GetAwaiter().GetResult();
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            Payloads = new List<byte[]>();
        }

        [Benchmark]
        public async Task Benchmark_Delivery_PublishManyAsync()
        {
            await _deliveryService.PublishManyAsync(ExchangeName, QueueName, Payloads, false, null);
        }
    }
}
