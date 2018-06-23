using CookedRabbit.Core.Library.Services;

namespace CookedRabbit.Core.Library.Bus
{
    public class RabbitBus : IRabbitBus
    {
        private readonly IRabbitService _rabbitService;

        public RabbitBus(IRabbitService rabbitService)
        {
            _rabbitService = rabbitService;
        }
    }
}
