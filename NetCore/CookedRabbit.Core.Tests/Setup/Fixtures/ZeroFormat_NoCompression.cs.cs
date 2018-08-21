using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Services;
using System;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Tests.Fixtures
{
    [CollectionDefinition("IntegrationTests_NoCompression_ZeroFormat")]
    public class IntegrationCollection_NoCompression_ZeroFormat : ICollectionFixture<ZeroFormat_NoCompression>
    { }

    public class ZeroFormat_NoCompression : IDisposable
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

        public ZeroFormat_NoCompression()
        {
            Seasoning = new RabbitSeasoning
            {
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false
            };

            Seasoning.SerializeSettings.SerializationMethod = SerializationMethod.ZeroFormat;
            Seasoning.FactorySettings.RabbitHostName = "localhost";
            Seasoning.PoolSettings.EnableAutoScaling = true;
            Seasoning.PoolSettings.ConnectionName = "RabbitServiceTest";
            Seasoning.PoolSettings.ConnectionPoolCount = 1;
            Seasoning.PoolSettings.ChannelPoolCount = 2;

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

                    RabbitDeliveryService.Dispose();
                    RabbitTopologyService.Dispose();
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
