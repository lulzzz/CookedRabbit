using CookedRabbit.Core.Library.Services;

namespace CookedRabbit.Core.Library.Bus
{
    /// <summary>
    /// Not used yet.
    /// </summary>
    public class RabbitBus : IRabbitBus
    {
        private readonly IRabbitDeliveryService _rabbitDeliveryService;

        /// <summary>
        /// RabbitBus constructor.
        /// </summary>
        /// <param name="rabbitService"></param>
        public RabbitBus(IRabbitDeliveryService rabbitService)
        {
            _rabbitDeliveryService = rabbitService;
        }
    }
}
