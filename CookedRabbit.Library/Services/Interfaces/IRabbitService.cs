using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// The interface for RabbitService.
    /// </summary>
    public interface IRabbitService
    {
        /// <summary>
        /// Publish messages asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="payload"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        Task<bool> PublishAsync(string exchangeName, string queueName, byte[] payload, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously. When payload count exceeds a certain threshold (determined by your systems performance) consider using PublishManyInBatchesAsync().
        /// <para>Returns a List of the indices that failed to publish for calling service/methods to retry.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="payloads"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        Task<List<int>> PublishManyAsync(string exchangeName, string queueName, List<byte[]> payloads, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes.
        /// <para>Returns a List of the indices that failed to publish for calling service/methods to retry.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="payloads"></param>
        /// <param name="batchSize"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        Task<List<int>> PublishManyAsBatchesAsync(string exchangeName, string queueName, List<byte[]> payloads, ushort batchSize = 100, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes. High performance but experimental. Does not log exceptions.
        /// <para>Returns a List of the indices that failed to publish for calling service/methods to retry.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="payloads"></param>
        /// <param name="batchSize"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        Task<List<int>> PublishManyAsBatchesInParallelAsync(string exchangeName, string queueName, List<byte[]> payloads, ushort batchSize = 100, IBasicProperties messageProperties = null);


        /// <summary>
        /// Compresses the payload before doing performing similar steps to PublishAsync().
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="payload"></param>
        /// <param name="contentType"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        Task<bool> CompressAndPublishAsync(string exchangeName, string queueName, byte[] payload, string contentType, IBasicProperties messageProperties = null);


        /// <summary>
        /// Get a BasicGetResult from a queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task<BasicGetResult> GetAsync(string queueName);

        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a List&lt;BasicGetResult&gt;.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns></returns>
        Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount);


        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a List&lt;ValueTuple(IModel, BasicGetResult)&gt;.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task<(IModel Channel, BasicGetResult Result)> GetWithManualAckAsync(string queueName);

        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a ValueTuple(IModel, List&lt;BasicGetResult&gt;).</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns></returns>
        Task<(IModel Channel, List<BasicGetResult> Results)> GetManyWithManualAckAsync(string queueName, int batchCount);


        /// <summary>
        /// Get an AckableResult from a queue.
        /// <para>Returns a CookedRabbit AckableResult.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task<AckableResult> GetAckableAsync(string queueName);

        /// <summary>
        /// Get an AckableResult from a queue.
        /// <para>Returns a CookedRabbit AckableResult.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns></returns>
        Task<AckableResult> GetManyAckableAsync(string queueName, int batchCount);


        /// <summary>
        /// Gets a payload from a queue and decompresses.
        /// <para>Returns a byte[].</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task<byte[]> GetAndDecompressAsync(string queueName);


        /// <summary>
        /// Create a RabbitMQ Consumer asynchronously. (Requires EnableDispatchConsumersAsync = false in RabbitSeasoning.)
        /// </summary>
        /// <param name="ActionWork"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="autoAck"></param>
        /// <returns></returns>
        Task<EventingBasicConsumer> CreateConsumerAsync(Action<object, BasicDeliverEventArgs> ActionWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);

        /// <summary>
        /// Create a RabbitMQ AsyncConsumer asynchronously. (Requires EnableDispatchConsumersAsync = true in RabbitSeasoning.)
        /// </summary>
        /// <param name="AsyncWork"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="autoAck"></param>
        /// <returns></returns>
        Task<AsyncEventingBasicConsumer> CreateAsynchronousConsumerAsync(Func<object, BasicDeliverEventArgs, Task> AsyncWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);
    }
}
