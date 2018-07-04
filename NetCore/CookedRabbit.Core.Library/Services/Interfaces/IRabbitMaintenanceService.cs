using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Services
{
    /// <summary>
    /// CookedRabbit service for doing on the fly of maintenance.
    /// </summary>
    public interface IRabbitMaintenanceService
    {
        /// <summary>
        /// Empty the queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="deleteQueueAfter"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PurgeQueueAsync(string queueName, bool deleteQueueAfter = false);

        /// <summary>
        /// Transfers one message from one queue to another queue.
        /// </summary>
        /// <param name="originQueueName"></param>
        /// <param name="targetQueueName"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> TransferMessageAsync(string originQueueName, string targetQueueName);

        /// <summary>
        /// Transfers all messages from one queue to another queue. Stops when on the first null result.
        /// </summary>
        /// <param name="originQueueName"></param>
        /// <param name="targetQueueName"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> TransferAllMessageAsync(string originQueueName, string targetQueueName);
    }
}
