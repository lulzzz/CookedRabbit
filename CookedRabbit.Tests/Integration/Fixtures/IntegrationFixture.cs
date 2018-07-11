using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Services;
using System;
using Xunit;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integration.Fixtures
{
    [CollectionDefinition("IntegrationTests")]
    public class IntegrationCollection : ICollectionFixture<IntegrationFixture>
    { }

    public class IntegrationFixture : IDisposable
    {
        public RabbitDeliveryService RabbitDeliveryService { get; private set; }
        public RabbitTopologyService RabbitTopologyService { get; private set; }
        public RabbitSerializeService RabbitSerializeService { get; private set; }
        public RabbitSeasoning Seasoning { get; private set; }
        public string TestQueueName1 { get; private set; } = "CookedRabbit.TestQueue1";
        public string TestQueueName2 { get; private set; } = "CookedRabbit.TestQueue2";
        public string TestQueueName3 { get; private set; } = "CookedRabbit.TestQueue3";
        public string TestQueueName4 { get; private set; } = "CookedRabbit.TestQueue4";
        public string TestExchangeName { get; private set; } = "CookedRabbit.TestExchange";

        public IntegrationFixture()
        {
            Seasoning = new RabbitSeasoning
            {
                RabbitHostName = "localhost",
                ConnectionName = "RabbitServiceTest",
                ConnectionPoolCount = 1,
                ChannelPoolCount = 4,
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false
            };

            var channelPool = new RabbitChannelPool();
            channelPool
                .SetConnectionPoolAsync(Seasoning, new RabbitConnectionPool())
                .GetAwaiter().GetResult();


            RabbitDeliveryService = new RabbitDeliveryService(Seasoning, channelPool);
            RabbitTopologyService = new RabbitTopologyService(Seasoning, channelPool);
            RabbitSerializeService = new RabbitSerializeService(Seasoning, channelPool);

            try
            {
                RabbitTopologyService.QueueDeleteAsync(TestQueueName1, false, false).GetAwaiter().GetResult();
                RabbitTopologyService.QueueDeleteAsync(TestQueueName2, false, false).GetAwaiter().GetResult();
                RabbitTopologyService.QueueDeleteAsync(TestQueueName3, false, false).GetAwaiter().GetResult();
                RabbitTopologyService.QueueDeleteAsync(TestQueueName4, false, false).GetAwaiter().GetResult();
                RabbitTopologyService.ExchangeDeleteAsync(TestExchangeName, false).GetAwaiter().GetResult();
            }
            catch { }
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
                        RabbitTopologyService.QueueDeleteAsync(TestQueueName1, false, false).GetAwaiter().GetResult();
                        RabbitTopologyService.QueueDeleteAsync(TestQueueName2, false, false).GetAwaiter().GetResult();
                        RabbitTopologyService.QueueDeleteAsync(TestQueueName3, false, false).GetAwaiter().GetResult();
                        RabbitTopologyService.QueueDeleteAsync(TestQueueName4, false, false).GetAwaiter().GetResult();
                        RabbitTopologyService.ExchangeDeleteAsync(TestExchangeName, false).GetAwaiter().GetResult();
                    }
                    catch { }

                    RabbitDeliveryService.Dispose(true);
                    RabbitTopologyService.Dispose(true);
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
