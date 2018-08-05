using CookedRabbit.Core.Library.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Services
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
        /// Publish messages asynchronously.
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PublishAsync(Envelope envelope, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publish messages asynchronously.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PublishAsync(Envelope envelope);

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
        /// Publishes many messages asynchronously. When payload count exceeds a certain threshold (determined by your systems performance) consider using PublishManyInBatchesAsync().
        /// <para>Returns a List of the indices that failed to publish for calling service/methods to retry.</para>
        /// </summary>
        /// <param name="envelopes"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        Task<List<int>> PublishManyAsync(List<Envelope> envelopes, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously.
        /// </summary>
        /// <param name="letters"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        Task<List<int>> PublishManyAsync(List<Envelope> letters);

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
        /// Publishes many messages asynchronously in configurable batch sizes.
        /// </summary>
        /// <param name="envelopes"></param>
        /// <param name="batchSize"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        Task<List<int>> PublishManyAsBatchesAsync(List<Envelope> envelopes, int batchSize = 100,
            IBasicProperties messageProperties = null);

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes.
        /// <para>NOTICE: High performance but experimental. Does not report failures.</para>
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
        /// Publishes many messages asynchronously in configurable batch sizes.
        /// <para>NOTICE: High performance but experimental. Does not report failures.</para>
        /// </summary>
        /// <param name="envelopes"></param>
        /// <param name="batchSize"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        Task PublishManyAsBatchesInParallelAsync(List<Envelope> envelopes, int batchSize = 100,
            IBasicProperties messageProperties = null);

        /// <summary>
        /// Publish using BasicPublishBatch built-in to RabbitMQ 5.1.0+.
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="payloads"></param>
        /// <param name="mandatory"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PublishBasicBatchAsync(string exchangeName, string routingKey, List<byte[]> payloads,
            bool mandatory = false, IBasicProperties messageProperties = null);

        /// <summary>
        /// Publish using BasicPublishBatch built-in to RabbitMQ 5.1.0+.
        /// </summary>
        /// <param name="letters"></param>
        /// <returns>A bool indicating success or failure.</returns>
        Task<bool> PublishBasicBatchAsync(List<Envelope> letters);


        /// <summary>
        /// Get a BasicGetResult from a queue asynchronously.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>Returns a BasicGetResult (RabbitMQ).</returns>
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

        /// <summary>
        /// Get the number of times the channel pool autoscaled up.
        /// </summary>
        /// <returns></returns>
        long GetAutoScalingIterationCount();

        /// <summary>
        /// Get the number of times the ackable channel pool autoscaled up.
        /// </summary>
        /// <returns></returns>
        long GetAckableAutoScalingIterationCount();
    }
}
