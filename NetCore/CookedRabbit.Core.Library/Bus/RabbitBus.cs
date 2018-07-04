using CookedRabbit.Core.Library.Services;

namespace CookedRabbit.Core.Library.Bus
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
