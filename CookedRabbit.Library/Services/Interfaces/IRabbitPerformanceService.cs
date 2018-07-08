using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using System.Threading.Tasks;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for opinionated performance delivery and receive methods.
    /// </summary>
    public interface IRabbitPerformanceService
    {
        /// <summary>
        /// Serialize and publish a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="envelope"></param>
        /// <param name="serializationMethod"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> SerializeAndPublishAsync<T>(T message, Envelope envelope,
            SerializationMethod serializationMethod = SerializationMethod.Utf8Json);

        /// <summary>
        /// Get and deserialize a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="serializationMethod"></param>
        /// <returns>A an object of type T.</returns>
        Task<T> GetAndDeserializeAsync<T>(string queueName,
            SerializationMethod serializationMethod = SerializationMethod.Utf8Json);

        /// <summary>
        /// Publish messages asynchronously.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PublishAsync(Envelope envelope);

        /// <summary>
        /// Get messages asynchronously.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>A <see cref="RabbitMQ.Client.BasicGetResult"/></returns>
        Task<BasicGetResult> GetAsync(string queueName);
    }
}
