using CookedRabbit.Core.Library.Models;
using System;
using Xunit;
using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Tests.Fixtures
{
    [CollectionDefinition("Standard")]
    public class StandardCollection : ICollectionFixture<StandardFixture>
    { }

    public class StandardFixture : IDisposable
    {
        public RabbitBurrow Burrow { get; set; }
        public RabbitSeasoning Seasoning { get; private set; }
        public string TestQueueName1 { get; private set; } = "CookedRabbit.TestQueue1";
        public string TestQueueName2 { get; private set; } = "CookedRabbit.TestQueue2";
        public string TestQueueName3 { get; private set; } = "CookedRabbit.TestQueue3";
        public string TestQueueName4 { get; private set; } = "CookedRabbit.TestQueue4";
        public string TestExchangeName { get; private set; } = "CookedRabbit.TestExchange";

        public StandardFixture()
        {
            Seasoning = new RabbitSeasoning
            {
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false
            };

            Seasoning.SerializeSettings.SerializationMethod = SerializationMethod.Utf8Json;
            Seasoning.FactorySettings.RabbitHostName = "localhost";
            Seasoning.PoolSettings.EnableAutoScaling = true;
            Seasoning.PoolSettings.ConnectionName = "RabbitServiceTest";
            Seasoning.PoolSettings.ConnectionPoolCount = 5;
            Seasoning.PoolSettings.ChannelPoolCount = 25;

            Burrow = new RabbitBurrow(Seasoning);

            try
            {
                Burrow.Maintenance.QueueDeclareAsync(TestQueueName1).GetAwaiter().GetResult();
                Burrow.Maintenance.QueueDeclareAsync(TestQueueName2).GetAwaiter().GetResult();
                Burrow.Maintenance.QueueDeclareAsync(TestQueueName3).GetAwaiter().GetResult();
                Burrow.Maintenance.QueueDeclareAsync(TestQueueName4).GetAwaiter().GetResult();
                Burrow.Maintenance.ExchangeDeclareAsync(TestExchangeName, ExchangeType.Fanout.Description()).GetAwaiter().GetResult();
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
                        Burrow.Maintenance.QueueDeleteAsync(TestQueueName1, false, false).GetAwaiter().GetResult();
                        Burrow.Maintenance.QueueDeleteAsync(TestQueueName2, false, false).GetAwaiter().GetResult();
                        Burrow.Maintenance.QueueDeleteAsync(TestQueueName3, false, false).GetAwaiter().GetResult();
                        Burrow.Maintenance.QueueDeleteAsync(TestQueueName4, false, false).GetAwaiter().GetResult();
                        Burrow.Maintenance.ExchangeDeleteAsync(TestExchangeName, false).GetAwaiter().GetResult();
                    }
                    catch { }

                    Burrow.Transmission.Dispose();
                    Burrow.Maintenance.Dispose();
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
