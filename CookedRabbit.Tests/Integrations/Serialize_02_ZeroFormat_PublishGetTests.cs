using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Services;
using System.Threading.Tasks;
using Xunit;
using static CookedRabbit.Library.Utilities.Enums;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Tests.Integrations
{
    public class Serialize_01_ZeroFormat_PublishGetTests
    {
        private readonly IRabbitChannelPool _rchanp;
        private readonly IRabbitConnectionPool _rconp;
        private readonly RabbitSerializeService _rabbitSerializeService;
        private readonly RabbitTopologyService _rabbitTopologyService;
        private readonly RabbitSeasoning _seasoning;
        private readonly string _testQueueName = "CookedRabbit.RabbitServiceTestQueue";
        private readonly string _testExchangeName = "CookedRabbit.RabbitServiceTestExchange";

        public Serialize_01_ZeroFormat_PublishGetTests()
        {
            _seasoning = new RabbitSeasoning
            {
                RabbitHost = "localhost",
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
    }
}
