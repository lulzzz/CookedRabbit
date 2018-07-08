using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for opinionated performance serialization and receive methods.
    /// </summary>
    public interface IRabbitSerializeService
    {
        /// <summary>
        /// Serialize and publish a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> SerializeAndPublishAsync<T>(T message, Envelope envelope);

        /// <summary>
        /// Serialize and publish messages asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages">The messages that will go in letters.</param>
        /// <param name="envelopeTemplate">Envelope to use as a template for creating letters.</param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        Task<List<int>> SerializeAndPublishManyAsync<T>(List<T> messages, Envelope envelopeTemplate);

        /// <summary>
        /// Compress and Publish a message asynchronously (compression needs to be enabled). Envelope body cannot be null.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> CompressAndPublishAsync(Envelope envelope);

        /// <summary>
        /// Get and deserialize a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <returns>An object of type T.</returns>
        Task<T> GetAndDeserializeAsync<T>(string queueName);

        /// <summary>
        /// Get many messages and deserialize the messages asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>An object of type T.</returns>
        Task<List<T>> GetAndDeserializeManyAsync<T>(string queueName, int batchCount);

        /// <summary>
        /// Gets and decompresses a message body/payload asynchronously.
        /// <para>Service needs to have CompressionEnabled:true and CompressionMethod:method needs to match what was placed in the queue.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task<byte[]> GetAndDecompressAsync(string queueName);


        /// <summary>
        /// Publish messages asynchronously.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PublishAsync(Envelope envelope);

        /// <summary>
        /// Publishes many messages asynchronously.
        /// </summary>
        /// <param name="letters"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        Task<List<int>> PublishManyAsync(List<Envelope> letters);

        /// <summary>
        /// Get messages asynchronously.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>A <see cref="RabbitMQ.Client.BasicGetResult"/></returns>
        Task<BasicGetResult> GetAsync(string queueName);

        /// <summary>
        /// Get a List of BasicGetResult from a queue asynchronously up to the batch count.
        /// </summary>
        /// <param name="queueName">The queue to get messages from.</param>
        /// <param name="batchCount">Limits the number of results to acquire.</param>
        /// <returns>A List of <see cref="RabbitMQ.Client.BasicGetResult"/>.</returns>
        Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount);

        /// <summary>
        /// Gets all messages from a queue asynchronously. Stops on empty queue or on first error.
        /// </summary>
        /// <param name="queueName">The queue to get messages from.</param>
        /// <returns>A List of <see cref="RabbitMQ.Client.BasicGetResult"/>.</returns>
        Task<List<BasicGetResult>> GetAllAsync(string queueName);
    }
}
