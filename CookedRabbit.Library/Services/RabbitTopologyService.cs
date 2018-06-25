using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;

namespace CookedRabbit.Library.Services
{
    public class RabbitTopologyService : IRabbitTopologyService
    {
        private readonly RabbitChannelPool _rcp = null;
        private readonly RabbitTopologySeasoning _originalRabbitSeasoning = null; // Used if for recovery later.

        public RabbitTopologyService(RabbitTopologySeasoning rabbitSeasoning)
        {
            _originalRabbitSeasoning = rabbitSeasoning;
            _rcp = RabbitChannelPool.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }
    }
}
