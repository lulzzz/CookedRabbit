using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Utilities
{
    /// <summary>
    /// CookedRabbit Factories helps create and initialize various CookedRabbit objects.
    /// </summary>
    public static class Factories
    {
        /// <summary>
        /// CookedRabbit RabbitChannelPool factory.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <returns><see cref="IRabbitChannelPool"/></returns>
        public static async Task<IRabbitChannelPool> CreateRabbitChannelPoolAsync(RabbitSeasoning rabbitSeasoning)
        {
            RabbitChannelPool rcp = new RabbitChannelPool();
            await rcp.Initialize(rabbitSeasoning);
            return rcp;
        }

        /// <summary>
        /// CookedRabbit RabbitConnectionPool factory.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <returns><see cref="IRabbitConnectionPool"/></returns>
        public static async Task<IRabbitConnectionPool> CreateRabbitConnectionPoolAsync(RabbitSeasoning rabbitSeasoning)
        {
            RabbitConnectionPool rcp = new RabbitConnectionPool();
            await rcp.Initialize(rabbitSeasoning);
            return rcp;
        }
    }
}
