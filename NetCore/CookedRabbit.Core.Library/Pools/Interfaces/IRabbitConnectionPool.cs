using RabbitMQ.Client;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Pools
{
    /// <summary>
    /// CookedRabbit RabbitConnectionPool creates the connections and manages the connection usage.
    /// </summary>
    public interface IRabbitConnectionPool
    {
        /// <summary>
        /// Public method to get a connection manually. Lifetime is the responsiblity of the calling service.
        /// </summary>
        /// <returns>Returns an IConnection (RabbitMQ).</returns>
        IConnection GetConnection();

        /// <summary>
        /// RabbitConnectionPool shutdown method closes all connections and disposes them.
        /// </summary>
        void Shutdown();
    }
}
