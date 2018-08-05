using BenchmarkDotNet.Attributes;
using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Benchmark.Tests
{
    public class AutoScaleBenchmark
    {
        RabbitTopologyService _topologyService;
        RabbitDeliveryService _deliveryService;

        public AutoScaleBenchmark()
        { }

        [GlobalSetup]
        public void Setup()
        {
            // Configured for performance.
            var seasoning = new RabbitSeasoning
            {
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false,
                WriteErrorsToConsole = false,
                WriteErrorsToILogger = false,
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
        }

        [Params(1, 5, 10, 50, 100)]
        public int MessagesToSend { get; set; }

        [Benchmark]
        public async Task Benchmark_AutoScaleDisabled_PublishMessagesToLocalHostAsync()
        {
            var payloads = await CreatePayloadsAsync(MessagesToSend, 200);

            await PublishMessagesToLocalHostAsync(MessagesToSend, payloads);
        }

        public async Task PublishMessagesToLocalHostAsync(int messagesToSend, List<byte[]> payloads)
        {
            var queueName1 = "CookedRabbit.Benchmark.Scaling1";
            var queueName2 = "CookedRabbit.Benchmark.Scaling2";
            var queueName3 = "CookedRabbit.Benchmark.Scaling3";
            var queueName4 = "CookedRabbit.Benchmark.Scaling4";
            var queueName5 = "CookedRabbit.Benchmark.Scaling5";

            var exchangeName = string.Empty;

            var createSuccess1 = await _topologyService.QueueDeclareAsync(queueName1);
            var createSuccess2 = await _topologyService.QueueDeclareAsync(queueName2);
            var createSuccess3 = await _topologyService.QueueDeclareAsync(queueName3);
            var createSuccess4 = await _topologyService.QueueDeclareAsync(queueName4);
            var createSuccess5 = await _topologyService.QueueDeclareAsync(queueName5);

            var task1 = _deliveryService.PublishManyAsync(exchangeName, queueName1, payloads, false, null);
            var task2 = _deliveryService.PublishManyAsync(exchangeName, queueName2, payloads, false, null);
            var task3 = _deliveryService.PublishManyAsync(exchangeName, queueName3, payloads, false, null);
            var task4 = _deliveryService.PublishManyAsync(exchangeName, queueName4, payloads, false, null);
            var task5 = _deliveryService.PublishManyAsync(exchangeName, queueName5, payloads, false, null);

            await Task.WhenAll(new Task[] { task1, task2, task3, task4, task5 });
        }
    }
}
