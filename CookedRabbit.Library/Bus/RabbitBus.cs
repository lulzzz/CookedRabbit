using CookedRabbit.Library.Services;

namespace CookedRabbit.Library.Bus
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
