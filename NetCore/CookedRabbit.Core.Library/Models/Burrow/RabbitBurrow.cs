using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Services;
using CookedRabbit.Core.Library.Utilities;
using Microsoft.Extensions.Logging;

namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// A RabbitBurrow contains all the pre-built paths for Rabbits to take.
    /// </summary>
    public class RabbitBurrow
    {
        private readonly RabbitSeasoning _seasoning;

        /// <summary>
        /// CookedRabbit RabbitSerializeService for Serilization, Compression, and Delivery methods.
        /// </summary>
        public IRabbitSerializeService Transmission;
        /// <summary>
        /// CookedRabbit RabbitMaintenanceService for Queue/Exchange Management and Topology methods.
        /// </summary>
        public IRabbitMaintenanceService Maintenance;

        /// <summary>
        /// CookedRabbit RabbitBurrow constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitBurrow(RabbitSeasoning rabbitSeasoning, ILogger logger = null)
        {
            _seasoning = rabbitSeasoning;

            var channelPool = Factories.CreateRabbitChannelPoolAsync(_seasoning).GetAwaiter().GetResult();

            Transmission = new RabbitSerializeService(_seasoning, channelPool, logger);
            Maintenance = new RabbitMaintenanceService(_seasoning, channelPool, logger);
        }

        /// <summary>
        /// CookedRabbit RabbitBurrow constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <param name="logger"></param>
        public RabbitBurrow(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null)
        {
            _seasoning = rabbitSeasoning;

            Transmission = new RabbitSerializeService(_seasoning, rcp, logger);
            Maintenance = new RabbitMaintenanceService(_seasoning, rcp, logger);
        }
    }
}
