using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Services;
using System;
using System.Collections.Generic;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Tests.Fixtures
{
    [CollectionDefinition("BenchmarkFixture")]
    public class BenchmarkCollection : ICollectionFixture<BenchmarkFixture>
    { }

    public class BenchmarkFixture : IDisposable
    {
        public RabbitTopologyService TopologyService;
        public RabbitDeliveryService DeliveryService;
        public RabbitMaintenanceService MaintenanceService;

        public List<byte[]> Payloads { get; set; }

        public string QueueName { get; set; }
        public string ExchangeName { get; set; }

        public int MessagesToSend { get; set; } = 100;
        public int MessageSize { get; set; } = 100;

        public BenchmarkFixture()
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
            seasoning.PoolSettings.ChannelPoolCount = 4;

            var channelPool = new RabbitChannelPool();
            channelPool.Initialize(seasoning).GetAwaiter().GetResult();

            DeliveryService = new RabbitDeliveryService(seasoning, channelPool);
            TopologyService = new RabbitTopologyService(seasoning, channelPool);
            MaintenanceService = new RabbitMaintenanceService(seasoning, channelPool);

            TopologyService.QueueDeclareAsync(QueueName).GetAwaiter().GetResult();
            Payloads = CreatePayloadsAsync(MessagesToSend, MessageSize).GetAwaiter().GetResult();
        }

        #region Dispose Section

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Cleanup
                    try
                    {
                        MaintenanceService.PurgeQueueAsync(QueueName).GetAwaiter().GetResult();
                    }
                    catch { }

                    TopologyService.Dispose(true);
                    DeliveryService.Dispose(true);
                    MaintenanceService.Dispose(true);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
