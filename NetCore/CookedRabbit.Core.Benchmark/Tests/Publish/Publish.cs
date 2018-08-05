using BenchmarkDotNet.Attributes;
using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Services;
using CookedRabbit.Core.Library.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Benchmark.Tests
{
    public class Publish
    {
        private RabbitTopologyService _topologyService;
        private RabbitDeliveryService _deliveryService;
        private RabbitMaintenanceService _maintenanceService;

        private List<byte[]> Payloads { get; set; }

        private string QueueName = string.Empty;
        private string ExchangeName = string.Empty;

        [Params(1, 5, 10)]
        public int MessagesToSend { get; set; }
        [Params(100, 200, 500)]
        public int MessageSizes { get; set; }

        [IterationSetup]
        public void IterationSetup()
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

            var channelPool = Factories.CreateRabbitChannelPoolAsync(seasoning).GetAwaiter().GetResult();

            _deliveryService = new RabbitDeliveryService(seasoning, channelPool);
            _topologyService = new RabbitTopologyService(seasoning, channelPool);
            _maintenanceService = new RabbitMaintenanceService(seasoning, channelPool);

            _topologyService.QueueDeclareAsync(QueueName).GetAwaiter().GetResult();
            _maintenanceService.PurgeQueueAsync(QueueName).GetAwaiter().GetResult();

            Payloads = CreatePayloadsAsync(MessagesToSend, MessageSizes).GetAwaiter().GetResult();
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _maintenanceService.PurgeQueueAsync(QueueName).GetAwaiter().GetResult();
        }

        [Benchmark]
        public async Task Benchmark_Delivery_PublishManyAsync()
        {
            await _deliveryService.PublishManyAsync(ExchangeName, QueueName, Payloads, false, null);
        }
    }
}
