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
    /// CookedRabbit RabbitDeliveryService delivers and receives messages using RabbitMQ.
    /// </summary>
    public class RabbitDeliveryService : RabbitBaseService, IRabbitDeliveryService, IDisposable
    {
        #region Constructor Section

        /// <summary>
        /// CookedRabbit RabbitDeliveryService constructor.
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
        /// CookedRabbit RabbitDeliveryService constructor. Allows for the sharing of a channel pool. If channel pool is not initialized, it will automatically initialize in here.
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
        /// CookedRabbit RabbitDeliveryService constructor. Allows for the sharing of a channel pool and connection pool.
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

        #endregion

        #region BasicPublish Section

        /// <summary>
        /// Publish messages asynchronously.
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
            if (payload is null) throw new ArgumentNullException(nameof(payload));

            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                    routingKey: routingKey,
                    mandatory: mandatory,
                    basicProperties: messageProperties,
                    body: payload);

                success = true;
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
        /// Publish messages asynchronously.
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> PublishAsync(Envelope envelope, IBasicProperties messageProperties = null)
        {
            if (envelope is null) throw new ArgumentNullException(nameof(envelope));
            if (envelope.MessageBody is null) throw new ArgumentNullException(nameof(envelope.MessageBody));

            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.BasicPublish(exchange: envelope.ExchangeName ?? string.Empty,
                    routingKey: envelope.RoutingKey,
                    mandatory: envelope.Mandatory,
                    basicProperties: messageProperties,
                    body: envelope.MessageBody);

                success = true;
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
        /// Publish messages asynchronously.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> PublishAsync(Envelope envelope)
        {
            if (envelope is null) throw new ArgumentNullException(nameof(envelope));
            if (envelope.MessageBody is null) throw new ArgumentNullException(nameof(envelope.MessageBody));

            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                var messageProperties = channelPair.Channel.CreateBasicProperties();
                messageProperties.ContentEncoding = envelope.ContentEncoding.Description();
                messageProperties.ContentType = envelope.MessageType;

                channelPair.Channel.BasicPublish(exchange: envelope.ExchangeName ?? string.Empty,
                        routingKey: envelope.RoutingKey,
                        mandatory: envelope.Mandatory,
                        basicProperties: messageProperties,
                        body: envelope.MessageBody);

                success = true;
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
        /// Publish using BasicPublishBatch built-in to RabbitMQ 5.1.0+.
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="payloads"></param>
        /// <param name="mandatory"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> BasicPublishBatchAsync(string exchangeName, string routingKey, List<byte[]> payloads,
            bool mandatory = false, IBasicProperties messageProperties = null)
        {
            if (payloads is null) throw new ArgumentNullException(nameof(payloads));

            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                var batch = channelPair.Channel.CreateBasicPublishBatch();

                await Task.Run(() =>
                {
                    for (int i = 0; i < payloads.Count; i++)
                    {
                        batch.Add(exchangeName, routingKey, mandatory, messageProperties, payloads[i]);
                    }

                    batch.Publish();
                });

                success = true;
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
        /// Publish using BasicPublishBatch built-in to RabbitMQ 5.1.0+.
        /// </summary>
        /// <param name="letters"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> PublishBasicBatchAsync(List<Envelope> letters)
        {
            if (letters is null) throw new ArgumentNullException(nameof(letters));

            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var messageProperties = channelPair.Channel.CreateBasicProperties();

            try
            {
                var batch = channelPair.Channel.CreateBasicPublishBatch();

                await Task.Run(() =>
                {
                    for (int i = 0; i < letters.Count; i++)
                    {
                        if (i == 0)
                        {
                            messageProperties.ContentEncoding = letters[i].ContentEncoding.Description();
                            messageProperties.ContentType = letters[i].MessageType;
                        }

                        batch.Add(letters[i].ExchangeName, letters[i].RoutingKey, letters[i].Mandatory, messageProperties, letters[i].MessageBody);
                    }

                    batch.Publish();
                });

                success = true;
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
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
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return result;
        }

        /// <summary>
        /// Get a List of BasicGetResult from a queue asynchronously up to the batch count.
        /// </summary>
        /// <param name="queueName">The queue to get messages from.</param>
        /// <param name="batchCount">Limits the number of results to acquire.</param>
        /// <returns>A List of <see cref="RabbitMQ.Client.BasicGetResult"/>.</returns>
        public async Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount)
        {
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var queueCount = 0U;

            try { queueCount = channelPair.Channel.MessageCount(queueName); }
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
                    catch (Exception e)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(Rand.Next(0, 2)); }
                }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return results;
        }

        /// <summary>
        /// Gets all messages from a queue asynchronously. Stops on empty queue or on first error.
        /// </summary>
        /// <param name="queueName">The queue to get messages from.</param>
        /// <returns>A List of <see cref="RabbitMQ.Client.BasicGetResult"/>.</returns>
        public async Task<List<BasicGetResult>> GetAllAsync(string queueName)
        {
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var results = new List<BasicGetResult>();

            while (true)
            {
                try
                {
                    var result = channelPair.Channel.BasicGet(queue: queueName, autoAck: true);
                    if (result == null) //Empty Queue
                    { break; }

                    results.Add(result);
                }
                catch (Exception e) when (results.Count() > 0)
                {
                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    break; // Does not throw so you can use the results already found.
                }
                catch (Exception e)
                {
                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                    else { break; }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(Rand.Next(0, 2)); }
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

        #endregion

        #region Custom Batch Publish Section

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
            if (payloads is null) throw new ArgumentNullException(nameof(payloads));

            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            for (int i = 0; i < payloads.Count; i++)
            {
                try
                {
                    channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                        routingKey: routingKey,
                        mandatory: mandatory,
                        basicProperties: messageProperties,
                        body: payloads[i]);
                }
                catch (Exception e)
                {
                    failures.Add(i);

                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                    else if (_seasoning.BatchBreakOnException) { break; }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(Rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

        /// <summary>
        /// Publishes many messages asynchronously. When payload count exceeds a certain threshold (determined by your systems performance) consider using PublishManyInBatchesAsync().
        /// <para>Returns a List of the indices that failed to publish for calling service/methods to retry.</para>
        /// </summary>
        /// <param name="letters"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        public async Task<List<int>> PublishManyAsync(List<Envelope> letters, IBasicProperties messageProperties = null)
        {
            if (letters is null) throw new ArgumentNullException(nameof(letters));

            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            for (int i = 0; i < letters.Count; i++)
            {
                try
                {
                    channelPair.Channel.BasicPublish(exchange: letters[i].ExchangeName ?? string.Empty,
                        routingKey: letters[i].RoutingKey,
                        mandatory: letters[i].Mandatory,
                        basicProperties: messageProperties,
                        body: letters[i].MessageBody);
                }
                catch (Exception e)
                {
                    failures.Add(i);

                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                    else if (_seasoning.BatchBreakOnException) { break; }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(Rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

        /// <summary>
        /// Publishes many messages asynchronously.
        /// </summary>
        /// <param name="letters"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        public async Task<List<int>> PublishManyAsync(List<Envelope> letters)
        {
            if (letters is null) throw new ArgumentNullException(nameof(letters));

            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            var messageProperties = channelPair.Channel.CreateBasicProperties();

            for (int i = 0; i < letters.Count; i++)
            {
                if (i == 0)
                {
                    messageProperties.ContentEncoding = letters[i].ContentEncoding.Description();
                    messageProperties.ContentType = letters[i].MessageType;
                }

                try
                {
                    channelPair.Channel.BasicPublish(exchange: letters[i].ExchangeName ?? string.Empty,
                        routingKey: letters[i].RoutingKey,
                        mandatory: letters[i].Mandatory,
                        basicProperties: messageProperties,
                        body: letters[i].MessageBody);
                }
                catch (Exception e)
                {
                    failures.Add(i);

                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                    else if (_seasoning.BatchBreakOnException) { break; }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(Rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes.
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
            if (payloads is null) throw new ArgumentNullException(nameof(payloads));

            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            while (payloads.Any())
            {
                var currentBatchSize = payloads.Count > batchSize ? batchSize : payloads.Count;
                var processingPayloads = payloads.Take(currentBatchSize).ToList();
                payloads.RemoveRange(0, currentBatchSize);

                for (int i = 0; i < processingPayloads.Count; i++)
                {
                    try
                    {
                        channelPair.Channel.BasicPublish(exchange: exchangeName ?? string.Empty,
                            routingKey: routingKey,
                            mandatory: mandatory,
                            basicProperties: messageProperties,
                            body: processingPayloads[i]);
                    }
                    catch (Exception e)
                    {
                        failures.Add(i);

                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                        else if (_seasoning.BatchBreakOnException) { break; }
                    }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(Rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes.
        /// </summary>
        /// <param name="letters"></param>
        /// <param name="batchSize"></param>
        /// <param name="messageProperties"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        public async Task<List<int>> PublishManyAsBatchesAsync(List<Envelope> letters, int batchSize = 100,
            IBasicProperties messageProperties = null)
        {
            if (letters is null) throw new ArgumentNullException(nameof(letters));

            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var count = 0;

            while (letters.Any())
            {
                var currentBatchSize = letters.Count > batchSize ? batchSize : letters.Count;
                var processingLetters = letters.Take(currentBatchSize).ToList();
                letters.RemoveRange(0, currentBatchSize);

                for (int i = 0; i < processingLetters.Count; i++)
                {
                    try
                    {
                        channelPair.Channel.BasicPublish(exchange: letters[i].ExchangeName ?? string.Empty,
                            routingKey: letters[i].RoutingKey,
                            mandatory: letters[i].Mandatory,
                            basicProperties: messageProperties,
                            body: letters[i].MessageBody);
                    }
                    catch (Exception e)
                    {
                        failures.Add(count);

                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                        else if (_seasoning.BatchBreakOnException) { break; }
                    }

                    count++;
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(Rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

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
        public async Task PublishManyAsBatchesInParallelAsync(string exchangeName, string routingKey, List<byte[]> payloads, int batchSize = 100,
            bool mandatory = false, IBasicProperties messageProperties = null)
        {
            if (payloads is null) throw new ArgumentNullException(nameof(payloads));

            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

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
                            catch
                            { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                        });

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(Rand.Next(0, 2)); }
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
                        catch
                        { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                    }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(Rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);
        }

        /// <summary>
        /// Publishes many messages asynchronously in configurable batch sizes.
        /// <para>NOTICE: High performance but experimental. Does not report failures.</para>
        /// </summary>
        /// <param name="envelopes"></param>
        /// <param name="batchSize"></param>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        public async Task PublishManyAsBatchesInParallelAsync(List<Envelope> envelopes, int batchSize = 100,
            IBasicProperties messageProperties = null)
        {
            if (envelopes is null) throw new ArgumentNullException(nameof(envelopes));

            var failures = new ConcurrentBag<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            while (envelopes.Any())
            {
                var procCount = Environment.ProcessorCount;
                var currentBatchSize = envelopes.Count > batchSize ? batchSize : envelopes.Count;
                var processingEnvelopes = envelopes.Take(currentBatchSize).ToList();
                envelopes.RemoveRange(0, currentBatchSize);

                if (processingEnvelopes.Count() >= procCount)
                {
                    Parallel.ForEach(processingEnvelopes, new ParallelOptions { MaxDegreeOfParallelism = procCount },
                        (envelope) =>
                        {
                            try
                            {
                                channelPair.Channel.BasicPublish(exchange: envelope.ExchangeName ?? string.Empty,
                                    routingKey: envelope.RoutingKey,
                                    mandatory: envelope.Mandatory,
                                    basicProperties: messageProperties,
                                    body: envelope.MessageBody);
                            }
                            catch
                            { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                        });

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(Rand.Next(0, 2)); }
                }
                else
                {
                    foreach (var envelope in processingEnvelopes)
                    {
                        try
                        {
                            channelPair.Channel.BasicPublish(exchange: envelope.ExchangeName ?? string.Empty,
                                routingKey: envelope.RoutingKey,
                                mandatory: envelope.Mandatory,
                                basicProperties: messageProperties,
                                body: envelope.MessageBody);
                        }
                        catch
                        { _rcp.FlagDeadChannel(channelPair.ChannelId); }
                    }

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(Rand.Next(0, 2)); }
                }
            }

            _rcp.ReturnChannelToPool(channelPair);
        }

        #endregion

        #region Custom Batch Get Section

        /// <summary>
        /// Get an AckableResult from a queue.
        /// <para>Returns a CookedRabbit AckableResult.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>An AckableResult (CookedRabbit object).</returns>
        public async Task<AckableResult> GetManyAckableAsync(string queueName, int batchCount)
        {
            var channelPair = await _rcp.GetPooledChannelPairAckableAsync().ConfigureAwait(false); ;
            var queueCount = 0U;
            var resultCount = 0;
            var results = new List<BasicGetResult>();

            try { queueCount = channelPair.Channel.MessageCount(queueName); }
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
                    catch (Exception e)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                        else if (_seasoning.BatchBreakOnException) { break; }
                    }

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(Rand.Next(0, 2)); }
                }
            }

            _rcp.ReturnChannelToAckPool(channelPair);

            return new AckableResult { Channel = channelPair.Channel, Results = results };
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
            var channelPair = await _rcp.GetPooledChannelPairAckableAsync().ConfigureAwait(false);
            var queueCount = 0U;
            var resultCount = 0;
            var results = new List<BasicGetResult>();

            try { queueCount = channelPair.Channel.MessageCount(queueName); }
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
                    catch (Exception e)
                    {
                        await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                        if (_seasoning.ThrowExceptions) { throw; }
                    }

                    if (_seasoning.ThrottleFastBodyLoops)
                    { await Task.Delay(Rand.Next(0, 2)); }
                }
            }

            _rcp.ReturnChannelToAckPool(channelPair);

            return (channelPair.Channel, results);
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
            if (_seasoning.FactorySettings.EnableDispatchConsumersAsync)
            { throw new ArgumentException("EnableDispatchConsumerAsync is set to true, set it to false to get an regular Consumer."); }

            var channel = await _rcp.GetTransientChannelAsync(enableAck: autoAck);
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
        /// Create a RabbitMQ Consumer (subscriber) asynchronously. (Requires EnableDispatchConsumersAsync = false in RabbitSeasoning.)
        /// <para>Usage: Wire up a ReceivedEvent to Consumer, then Channel needs to execute BasicConsume.</para>
        /// </summary>
        /// <param name="autoAck"></param>
        /// <returns>A RabbitMQ EventingBasicConsumer.</returns>
        public async Task<(EventingBasicConsumer Consumer, IModel Channel)> CreateConsumerAsync(bool autoAck = false)
        {
            if (_seasoning.FactorySettings.EnableDispatchConsumersAsync)
            { throw new ArgumentException("EnableDispatchConsumerAsync is set to true, set it to false to get an regular Consumer."); }

            var channel = await _rcp.GetTransientChannelAsync(enableAck: autoAck);
            if (channel is null) throw new Exception("Channel was unable to be created for this consumer.");

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(_seasoning.QosPrefetchSize, _seasoning.QosPrefetchCount, false);

            return (consumer, channel);
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
            if (!_seasoning.FactorySettings.EnableDispatchConsumersAsync)
            { throw new ArgumentException("EnableDispatchConsumerAsync is set to false, set it to true to get an AsyncConsumer."); }

            var channel = await _rcp.GetTransientChannelAsync(enableAck: autoAck);
            if (channel is null) throw new Exception("Channel was unable to be created for this consumer.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            channel.BasicQos(_seasoning.QosPrefetchSize, _seasoning.QosPrefetchCount, false);

            consumer.Received += (model, ea) => AsyncWork(model, ea);
            channel.BasicConsume(queue: queueName,
                                 autoAck: autoAck,
                                 consumer: consumer);

            return consumer;
        }

        /// <summary>
        /// Create a RabbitMQ AsyncConsumer (subscriber) asynchronously. (Requires EnableDispatchConsumersAsync = true in RabbitSeasoning.)
        /// <para>Usage: Wire up a ReceivedEvent to Consumer, then Channel needs to execute BasicConsume.</para>
        /// </summary>
        /// <param name="autoAck"></param>
        /// <returns>A RabbitMQ AsyncEventingBasicConsumer.</returns>
        public async Task<(AsyncEventingBasicConsumer Consumer, IModel Channel)> CreateAsynchronousConsumerAsync(bool autoAck = false)
        {
            if (!_seasoning.FactorySettings.EnableDispatchConsumersAsync)
            { throw new ArgumentException("EnableDispatchConsumerAsync is set to false, set it to true to get an AsyncConsumer."); }

            var channel = await _rcp.GetTransientChannelAsync(enableAck: autoAck);
            if (channel is null) throw new Exception("Channel was unable to be created for this consumer.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            channel.BasicQos(_seasoning.QosPrefetchSize, _seasoning.QosPrefetchCount, false);

            return (consumer, channel);
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

        /// <summary>
        /// Get the number of times the channel pool auto-scaled up.
        /// </summary>
        /// <returns></returns>
        public long GetAutoScalingIterationCount()
        {
            return _rcp.GetChannelPoolAutoScalingIterationCount();
        }

        /// <summary>
        /// Get the number of times the ackable channel pool auto-scaled up.
        /// </summary>
        /// <returns></returns>
        public long GetAckableAutoScalingIterationCount()
        {
            return _rcp.GetChannelPoolAutoScalingIterationCount();
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
