using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Utilities;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit delivery service for delivering and receiving messages using RabbitMQ.
    /// </summary>
    public class RabbitDeliveryService : RabbitBaseService, IRabbitDeliveryService, IDisposable
    {
        /// <summary>
        /// CookedRabbit RabbitService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitDeliveryService(RabbitSeasoning rabbitSeasoning, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = Factories.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }

        /// <summary>
        /// CookedRabbit RabbitService constructor.  Allows for the sharing of a channel pool. If channel is not initialized, it will automatically initialize in here.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <param name="logger"></param>
        public RabbitDeliveryService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = rcp;

            if (!_rcp.IsInitialized)
            { _rcp.Initialize(rabbitSeasoning).GetAwaiter().GetResult(); }
        }

        /// <summary>
        /// CookedRabbit RabbitService constructor. Allows for the sharing of a channel pool and connection pool.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rchanp"></param>
        /// <param name="rconp"></param>
        /// <param name="logger"></param>
        public RabbitDeliveryService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rchanp, IRabbitConnectionPool rconp, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;

            rchanp.SetConnectionPoolAsync(rabbitSeasoning, rconp).GetAwaiter().GetResult();

            _rcp = rchanp;
        }

        #region BasicPublish Section

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
        public async Task<bool> PublishAsync(string exchangeName, string routingKey, byte[] payload,
            bool mandatory = false, IBasicProperties messageProperties = null)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                    routingKey: routingKey,
                    mandatory,
                    basicProperties: messageProperties,
                    body: payload);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

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
        public async Task<List<int>> PublishManyAsync(string exchangeName, string routingKey, List<byte[]> payloads,
            bool mandatory = false, IBasicProperties messageProperties = null)
        {
            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var rand = new Random();
            var count = 0;

            foreach (var payload in payloads)
            {
                try
                {
                    channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                        routingKey: routingKey,
                        mandatory,
                        basicProperties: messageProperties,
                        body: payload);
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                {
                    failures.Add(count);
                    await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                }
                catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
                {
                    failures.Add(count);
                    await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                }
                catch (Exception e)
                {
                    failures.Add(count);
                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                }

                count++;

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

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
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        public async Task<List<int>> PublishManyAsBatchesAsync(string exchangeName, string routingKey, List<byte[]> payloads, int batchSize = 100,
            bool mandatory = false, IBasicProperties messageProperties = null)
        {
            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var rand = new Random();
            var count = 0;

            while (payloads.Any())
            {
                var currentBatchSize = payloads.Count > batchSize ? batchSize : payloads.Count;
                var processingPayloads = payloads.Take(currentBatchSize).ToList();
                payloads.RemoveRange(0, currentBatchSize);

                foreach (var payload in processingPayloads)
                {
                    try
                    {
                        channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                            routingKey: routingKey,
                            mandatory,
                            basicProperties: messageProperties,
                            body: payload);
                    }
                    catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                    {
                        failures.Add(count);
                        await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
                    {
                        failures.Add(count);
                        await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (Exception e)
                    {
                        failures.Add(count);
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }

                    count++;
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

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
        public async Task PublishManyAsBatchesInParallelAsync(string exchangeName, string routingKey, List<byte[]> payloads, int batchSize = 100,
            bool mandatory = false, IBasicProperties messageProperties = null)
        {
            var failures = new ConcurrentBag<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var rand = new Random();

            while (payloads.Any())
            {
                var procCount = Environment.ProcessorCount;
                var currentBatchSize = payloads.Count > batchSize ? batchSize : payloads.Count;
                var processingPayloads = payloads.Take(currentBatchSize).ToList();
                payloads.RemoveRange(0, currentBatchSize);

                if (processingPayloads.Count() >= procCount)
                {
                    Parallel.ForEach(processingPayloads, new ParallelOptions { MaxDegreeOfParallelism = procCount },
                        (payload) =>
                        {
                            try
                            {
                                channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                                    routingKey: routingKey,
                                    mandatory,
                                    basicProperties: messageProperties,
                                    body: payload);
                            }
                            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
                            { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                            catch (Exception)
                            { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                        });

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(rand.Next(0, 2)); }
                }
                else
                {
                    foreach (var payload in processingPayloads)
                    {
                        try
                        {
                            channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                                routingKey: routingKey,
                                mandatory,
                                basicProperties: messageProperties,
                                body: payload);
                        }
                        catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
                        { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                        catch (Exception)
                        { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                    }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            var failureList = failures.ToList();
            failureList.Sort();
        }

        #endregion

        #region BasicGet Section

        /// <summary>
        /// Get a BasicGetResult from a queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>Returns a BasicGetResult (RabbitMQ).</returns>
        public async Task<BasicGetResult> GetAsync(string queueName)
        {
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            BasicGetResult result = null;

            try
            { result = channelPair.Channel.BasicGet(queue: queueName, autoAck: true); }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return result;
        }

        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a List&lt;BasicGetResult&gt;.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>A List of BasicGetResult (RabbitMQ object).</returns>
        public async Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount)
        {
            var rand = new Random();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var queueCount = 0U;

            try { queueCount = channelPair.Channel.MessageCount(queueName); }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            int resultCount = 0;
            var results = new List<BasicGetResult>();

            if (queueCount != 0)
            {
                while (queueCount > 0 && resultCount < batchCount)
                {
                    try
                    {
                        var result = channelPair.Channel.BasicGet(queue: queueName, autoAck: true);
                        if (result == null) //Empty Queue
                        { break; }

                        results.Add(result);
                        resultCount++;
                    }
                    catch (Exception e) when (results.Count() > 0)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        // Does not throw to use the results already found.
                        break;
                    }
                    catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                    {
                        await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
                    {
                        await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (Exception e)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(rand.Next(0, 2)); }
                }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return results;
        }

        #endregion

        #region BasicGet With Manual Ack Section

        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a List&lt;ValueTuple(IModel, BasicGetResult)&gt;.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>A ValueTuple(IModel, BasicGetResult) (RabbitMQ objects).</returns>
        public async Task<(IModel Channel, BasicGetResult Result)> GetWithManualAckAsync(string queueName)
        {
            var channelPair = await _rcp.GetPooledChannelPairAckableAsync().ConfigureAwait(false); ;

            BasicGetResult result = null;

            try
            { result = channelPair.Channel.BasicGet(queue: queueName, autoAck: false); }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            _rcp.ReturnChannelToAckPool(channelPair);

            return (channelPair.Channel, result);
        }

        /// <summary>
        /// Get a List of BasicGetResult from a queue.
        /// <para>Returns a ValueTuple(IModel, List&lt;BasicGetResult&gt;) (RabbitMQ objects).</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>A ValueTuple(IModel, List&lt;BasicGetResult&gt;) (RabbitMQ objects).</returns>
        public async Task<(IModel Channel, List<BasicGetResult> Results)> GetManyWithManualAckAsync(string queueName, int batchCount)
        {
            var rand = new Random();
            var channelPair = await _rcp.GetPooledChannelPairAckableAsync().ConfigureAwait(false);
            var queueCount = 0U;
            var resultCount = 0;
            var results = new List<BasicGetResult>();

            try { queueCount = channelPair.Channel.MessageCount(queueName); }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            if (queueCount != 0)
            {
                while (queueCount > 0 && resultCount < batchCount)
                {
                    try
                    {
                        var result = channelPair.Channel.BasicGet(queue: queueName, autoAck: false);
                        if (result == null) //Empty Queue
                        { break; }

                        results.Add(result);
                        resultCount++;
                    }
                    catch (Exception e) when (results.Count() > 0)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        // Does not throw to use the results already found.
                        break;
                    }
                    catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                    {
                        await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
                    {
                        await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (Exception e)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(rand.Next(0, 2)); }
                }
            }

            _rcp.ReturnChannelToAckPool(channelPair);

            return (channelPair.Channel, results);
        }

        /// <summary>
        /// Get an AckableResult from a queue.
        /// <para>Returns a CookedRabbit AckableResult.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>An AckableResult (CookedRabbit object).</returns>
        public async Task<AckableResult> GetAckableAsync(string queueName)
        {
            var channelPair = await _rcp.GetPooledChannelPairAckableAsync().ConfigureAwait(false); ;

            BasicGetResult result = null;

            try
            { result = channelPair.Channel.BasicGet(queue: queueName, autoAck: false); }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            _rcp.ReturnChannelToAckPool(channelPair);

            return new AckableResult { Channel = channelPair.Channel, Results = new List<BasicGetResult>() { result } };
        }

        /// <summary>
        /// Get an AckableResult from a queue.
        /// <para>Returns a CookedRabbit AckableResult.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>An AckableResult (CookedRabbit object).</returns>
        public async Task<AckableResult> GetManyAckableAsync(string queueName, int batchCount)
        {
            var rand = new Random();
            var channelPair = await _rcp.GetPooledChannelPairAckableAsync().ConfigureAwait(false); ;
            var queueCount = 0U;
            var resultCount = 0;
            var results = new List<BasicGetResult>();

            try { queueCount = channelPair.Channel.MessageCount(queueName); }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            if (queueCount != 0)
            {
                while (queueCount > 0 && resultCount < batchCount)
                {
                    try
                    {
                        var result = channelPair.Channel.BasicGet(queue: queueName, autoAck: false);
                        if (result == null) //Empty Queue
                        { break; }

                        results.Add(result);
                        resultCount++;
                    }
                    catch (Exception e) when (results.Count() > 0)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        // Does not throw to use the results already found.
                        break;
                    }
                    catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                    {
                        await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
                    {
                        await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }
                    catch (Exception e)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(rand.Next(0, 2)); }
                }
            }

            _rcp.ReturnChannelToAckPool(channelPair);

            return new AckableResult { Channel = channelPair.Channel, Results = results };
        }

        #endregion

        #region Consumer Section

        /// <summary>
        /// Create a RabbitMQ Consumer (subscriber) asynchronously. (Requires EnableDispatchConsumersAsync = false in RabbitSeasoning.)
        /// </summary>
        /// <param name="ActionWork"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="autoAck"></param>
        /// <returns>A RabbitMQ EventingBasicConsumer.</returns>
        public async Task<EventingBasicConsumer> CreateConsumerAsync(
            Action<object, BasicDeliverEventArgs> ActionWork,
            string queueName,
            ushort prefetchCount = 120,
            bool autoAck = false)
        {
            if (_seasoning.EnableDispatchConsumersAsync)
            { throw new ArgumentException("EnableDispatchConsumerAsync is set to true, set it to false to get an regular Consumer."); }

            var channel = await _rcp.GetTransientChannelAsync(enableAck: true);
            if (channel is null) throw new Exception("Channel was unable to be created for this consumer.");

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(_seasoning.QosPrefetchSize, _seasoning.QosPrefetchCount, false);
            consumer.Received += (model, ea) => ActionWork(model, ea);
            channel.BasicConsume(queue: queueName,
                                 autoAck: autoAck,
                                 consumer: consumer);

            return consumer;
        }

        /// <summary>
        /// Create a RabbitMQ AsyncConsumer (subscriber) asynchronously. (Requires EnableDispatchConsumersAsync = true in RabbitSeasoning.)
        /// </summary>
        /// <param name="AsyncWork"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="autoAck"></param>
        /// <returns>A RabbitMQ AsyncEventingBasicConsumer.</returns>
        public async Task<AsyncEventingBasicConsumer> CreateAsynchronousConsumerAsync(
            Func<object, BasicDeliverEventArgs, Task> AsyncWork,
            string queueName,
            ushort prefetchCount = 120,
            bool autoAck = false)
        {
            if (!_seasoning.EnableDispatchConsumersAsync)
            { throw new ArgumentException("EnableDispatchConsumerAsync is set to false, set it to true to get an AsyncConsumer."); }

            var channel = await _rcp.GetTransientChannelAsync(enableAck: true);
            if (channel is null) throw new Exception("Channel was unable to be created for this consumer.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            channel.BasicQos(_seasoning.QosPrefetchSize, _seasoning.QosPrefetchCount, false);
            consumer.Received += (model, ea) => AsyncWork(model, ea);
            channel.BasicConsume(queue: queueName,
                                 autoAck: autoAck,
                                 consumer: consumer);

            return consumer;
        }

        #endregion

        #region Miscellaneous Section

        /// <summary>
        /// Gets the total message count from a queue.
        /// </summary>
        /// <param name="queueName">The queue to check the message count.</param>
        /// <returns>A uint of the total messages in the queue.</returns>
        public async Task<uint> GetMessageCountAsync(string queueName)
        {
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            uint messageCount = 0;

            try
            { messageCount = channelPair.Channel.MessageCount(queueName); }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return messageCount;
        }

        #endregion

        #region Dispose Section

        private bool _disposedValue = false;

        /// <summary>
        /// RabbitService dispose method.
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { _rcp.Shutdown(); }
                _disposedValue = true;
            }
        }

        void IDisposable.Dispose() { Dispose(true); }

        #endregion
    }
}
