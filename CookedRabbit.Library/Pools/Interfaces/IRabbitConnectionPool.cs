using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Pools
{
    /// <summary>
    /// CookedRabbit RabbitConnectionPool creates the connections and manages the connection usage.
    /// </summary>
    public interface IRabbitConnectionPool
    {
        /// <summary>
        /// Check to see if the connection pool has already been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Public method to get a connection manually. Lifetime is the responsiblity of the calling service.
        /// </summary>
        /// <returns>Returns an IConnection (RabbitMQ).</returns>
        IConnection GetConnection();

        /// <summary>
        /// Initializes the RabbitConnectionPool for use.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <returns></returns>
        Task Initialize(RabbitSeasoning rabbitSeasoning);

        /// <summary>
        /// Allows for connections to be manually closed.
        /// </summary>
        void CloseConnections();

        /// <summary>
        /// RabbitConnectionPool shutdown method closes all connections and disposes them.
        /// </summary>
        void Shutdown();
    }
}
