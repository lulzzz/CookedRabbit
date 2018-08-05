using System.Threading.Tasks;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for doing on the fly of maintenance.
    /// </summary>
    public interface IRabbitMaintenanceService
    {
        /// <summary>
        /// Empty/purge the queue.
        /// </summary>
        /// <param name="queueName">The queue to remove from.</param>
        /// <param name="deleteQueueAfter">Indicate if you want to delete the queue after purge.</param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PurgeQueueAsync(string queueName, bool deleteQueueAfter = false);

        /// <summary>
        /// Transfers one message from one queue to another queue.
        /// </summary>
        /// <param name="originQueueName">The queue to remove from.</param>
        /// <param name="targetQueueName">The destination queue.</param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> TransferMessageAsync(string originQueueName, string targetQueueName);

        /// <summary>
        /// Transfers a number of messages from one queue to another queue.
        /// </summary>
        /// <param name="originQueueName">The queue to remove from.</param>
        /// <param name="targetQueueName">The destination queue.</param>
        /// <param name="count">Number of messages to transfer</param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> TransferMessagesAsync(string originQueueName, string targetQueueName, ushort count);

        /// <summary>
        /// Transfers all messages from one queue to another queue. Stops on the first null result (when queue is empty).
        /// </summary>
        /// <param name="originQueueName">The queue to remove from.</param>
        /// <param name="targetQueueName">The destination queue.</param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> TransferAllMessagesAsync(string originQueueName, string targetQueueName);

        /// <summary>
        /// Performs API Get calls based on the API target.
        /// </summary>
        /// <typeparam name="TResult">Internal model to be mapped to the specific API call result.</typeparam>
        /// <param name="rabbitApiTarget">Sepcific API endpoint.</param>
        /// <returns>TResult deserialized from JSON.</returns>
        Task<TResult> Api_GetAsync<TResult>(RabbitApiTarget rabbitApiTarget);
    }
}
