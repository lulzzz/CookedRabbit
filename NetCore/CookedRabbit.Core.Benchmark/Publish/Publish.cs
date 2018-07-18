using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Benchmark
{
    [CoreJob]
    public class Publish
    {
        private RabbitTopologyService _topologyService;
        private RabbitDeliveryService _deliveryService;
        private RabbitMaintenanceService _maintenanceService;

        private List<byte[]> Payloads { get; set; }

        private string QueueName = string.Empty;
        private string ExchangeName = string.Empty;

        private bool FirstRun = true;

        private async Task Setup(int messagesToSend, int messageSizes)
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
            await channelPool.Initialize(seasoning);

            _deliveryService = new RabbitDeliveryService(seasoning, channelPool);
            _topologyService = new RabbitTopologyService(seasoning, channelPool);
            _maintenanceService = new RabbitMaintenanceService(seasoning, channelPool);

            await _topologyService.QueueDeclareAsync(QueueName);

            Payloads = await CreatePayloadsAsync(messagesToSend, messageSizes);

            FirstRun = false;
        }

        [Benchmark]
        [Arguments(10, 100), Arguments(50, 100), Arguments(100, 100)]
        [Arguments(10, 200), Arguments(50, 200), Arguments(100, 200)]
        [Arguments(10, 500), Arguments(50, 500), Arguments(100, 500)]
        [Arguments(10, 1000), Arguments(50, 1000), Arguments(100, 1000)]
        public async Task Benchmark_Delivery_PublishManyAsync(int messagesToSend, int messageSizes)
        {
            if (FirstRun) { await Setup(messagesToSend, messageSizes); }

            var success = await _deliveryService.PublishManyAsync(ExchangeName, QueueName, Payloads, false, null);
            await _maintenanceService.PurgeQueueAsync(QueueName);
        }
    }
}
