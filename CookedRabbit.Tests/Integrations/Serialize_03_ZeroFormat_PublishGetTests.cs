using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Services;
using CookedRabbit.Tests.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integrations
{
    public class Serialize_03_ZeroFormat_PublishGetTests : IDisposable
    {
        private readonly IRabbitChannelPool _rchanp;
        private readonly IRabbitConnectionPool _rconp;
        private readonly RabbitSerializeService _rabbitSerializeService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName = "CookedRabbit.SerializeTestQueue";
        private readonly string _testExchangeName = "CookedRabbit.SerializeTestExchange";

        public Serialize_03_ZeroFormat_PublishGetTests()
        {
            _seasoning = new RabbitSeasoning
            {
                RabbitHostName = "localhost",
                ConnectionName = "RabbitSerializeServiceTest",
                ConnectionPoolCount = 1,
                ChannelPoolCount = 1,
                ThrottleFastBodyLoops = false,
                ThrowExceptions = false,
                SerializationMethod = SerializationMethod.ZeroFormat,
                CompressionEnabled = true,
                CompressionMethod = CompressionMethod.LZ4
            };

            _rchanp = new RabbitChannelPool();
            _rconp = new RabbitConnectionPool();

            _rchanp.SetConnectionPoolAsync(_seasoning, _rconp);

            _rabbitSerializeService = new RabbitSerializeService(_seasoning, _rchanp);
            _rabbitTopologyService = new RabbitTopologyService(_seasoning, _rchanp);

            try
            {
                _rabbitTopologyService.QueueDeleteAsync(_testQueueName, false, false).GetAwaiter().GetResult();
                _rabbitTopologyService.ExchangeDeleteAsync(_testExchangeName, false).GetAwaiter().GetResult();
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
                    { }
                    catch { }

                    _rabbitSerializeService.Dispose(true);
                    _rabbitTopologyService.Dispose(true);
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
