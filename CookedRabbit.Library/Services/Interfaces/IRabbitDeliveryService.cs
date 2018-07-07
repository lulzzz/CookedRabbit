using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// The interface for RabbitDeliveryService.
    /// </summary>
    public interface IRabbitDeliveryService
    {
        /// <summary>
        /// Publish messages asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName">The optional Exchange name.</param>
        /// <param name="routingKey">Either a topic/routing key or queue name.</param>
        /// <param name="payload"></param>
        /// <param name="mandatory"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PublishAsync(string exchangeName, string routingKey, byte[] payload,
            bool mandatory = false, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously. When payload count exceeds a certain threshold (determined by your systems performance) consider using PublishManyInBatchesAsync().
        /// <para>Returns a List of the indices that failed to publish for calling service/methods to retry.</para>
        /// </summary>
        /// <param name="exchangeName">The optional Exchange name.</param>
        /// <param name="routingKey">Either a topic/routing key or queue name.</param>
        /// <param name="payloads"></param>
        /// <param name="mandatory"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        Task<List<int>> PublishManyAsync(string exchangeName, string routingKey, List<byte[]> payloads, bool mandatory = false, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes.
        /// <para>Returns a List of the indices that failed to publish for calling service/methods to retry.</para>
        /// </summary>
        /// <param name="exchangeName">The optional Exchange name.</param>
        /// <param name="routingKey">Either a topic/routing key or queue name.</param>
        /// <param name="payloads"></param>
        /// <param name="batchSize"></param>
        /// <param name="mandatory"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A List&lt;int&gt; of the indices that failed to publish for calling service/methods to retry.</returns>
        Task<List<int>> PublishManyAsBatchesAsync(string exchangeName, string routingKey, List<byte[]> payloads, int batchSize = 100, bool mandatory = false, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes. High performance but experimental. Does not log exceptions.
        /// </summary>
        /// <param name="exchangeName">The optional Exchange name.</param>
        /// <param name="routingKey">Either a topic/routing key or queue name.</param>
        /// <param name="payloads"></param>
        /// <param name="batchSize"></param>
        /// <param name="mandatory"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        Task PublishManyAsBatchesInParallelAsync(string exchangeName, string routingKey, List<byte[]> payloads, int batchSize = 100, bool mandatory = false, IBasicProperties messageProperties = null);


        /// <summary>
        /// Compresses the payload before doing performing similar steps to PublishAsync().
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName">The optional Exchange name.</param>
        /// <param name="routingKey">Either a topic/routing key or queue name.</param>
        /// <param name="payload"></param>
        /// <param name="contentType"></param>
        /// <param name="mandatory"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> CompressAndPublishAsync(string exchangeName, string routingKey, byte[] payload, string contentType, bool mandatory = false, IBasicProperties messageProperties = null);


        /// <summary>
        /// Get a BasicGetResult from a queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>Returns a BasicGetResult (RabbitMQ).</returns>
        Task<BasicGetResult> GetAsync(string queueName);

        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a List&lt;BasicGetResult&gt;.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>A List of BasicGetResult (RabbitMQ object).</returns>
        Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount);


        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a List&lt;ValueTuple(IModel, BasicGetResult)&gt;.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>A ValueTuple(IModel, BasicGetResult) (RabbitMQ objects).</returns>
        Task<(IModel Channel, BasicGetResult Result)> GetWithManualAckAsync(string queueName);

        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a ValueTuple(IModel, List&lt;BasicGetResult&gt;) (RabbitMQ objects).</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>A ValueTuple(IModel, List&lt;BasicGetResult&gt;) (RabbitMQ objects).</returns>
        Task<(IModel Channel, List<BasicGetResult> Results)> GetManyWithManualAckAsync(string queueName, int batchCount);


        /// <summary>
        /// Get an AckableResult from a queue.
        /// <para>Returns a CookedRabbit AckableResult.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>An AckableResult (CookedRabbit object).</returns>
        Task<AckableResult> GetAckableAsync(string queueName);

        /// <summary>
        /// Get an AckableResult from a queue.
        /// <para>Returns a CookedRabbit AckableResult.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>An AckableResult (CookedRabbit object).</returns>
        Task<AckableResult> GetManyAckableAsync(string queueName, int batchCount);


        /// <summary>
        /// Gets a payload from a queue and decompresses.
        /// <para>Returns a byte[] (decompressed).</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>A byte[] (decompressed).</returns>
        Task<byte[]> GetAndDecompressAsync(string queueName);


        /// <summary>
        /// Create a RabbitMQ Consumer (subscriber) asynchronously. (Requires EnableDispatchConsumersAsync = false in RabbitSeasoning.)
        /// </summary>
        /// <param name="ActionWork"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="autoAck"></param>
        /// <returns>A RabbitMQ EventingBasicConsumer.</returns>
        Task<EventingBasicConsumer> CreateConsumerAsync(Action<object, BasicDeliverEventArgs> ActionWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);

        /// <summary>
        /// Create a RabbitMQ AsyncConsumer (subscriber) asynchronously. (Requires EnableDispatchConsumersAsync = true in RabbitSeasoning.)
        /// </summary>
        /// <param name="AsyncWork"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="autoAck"></param>
        /// <returns>A RabbitMQ AsyncEventingBasicConsumer.</returns>
        Task<AsyncEventingBasicConsumer> CreateAsynchronousConsumerAsync(Func<object, BasicDeliverEventArgs, Task> AsyncWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);

        /// <summary>
        /// Gets the total message count from a queue.
        /// </summary>
        /// <param name="queueName">The queue to check the message count.</param>
        /// <returns>A uint of the total messages in the queue.</returns>
        Task<uint> GetMessageCountAsync(string queueName);
    }
}
