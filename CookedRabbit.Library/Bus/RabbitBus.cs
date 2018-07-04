using CookedRabbit.Library.Services;

namespace CookedRabbit.Library.Bus
{
    /// <summary>
    /// Not used yet.
    /// </summary>
    public class RabbitBus : IRabbitBus
    {
        private readonly IRabbitDeliveryService _rabbitService;

        /// <summary>
        /// RabbitBus constructor.
        /// </summary>
        /// <param name="rabbitService"></param>
        public RabbitBus(IRabbitDeliveryService rabbitService)
        {
            _rabbitService = rabbitService;
        }
    }
}
