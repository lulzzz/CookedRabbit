using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Utilities;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CookedRabbit.Library.Utilities.Compression;
using static CookedRabbit.Library.Utilities.Enums;
using static CookedRabbit.Library.Utilities.Serialization;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for opinionated performance delivery and receive options.
    /// </summary>
    public class RabbitSerializeService : RabbitBaseService, IRabbitSerializeService, IDisposable
    {
        #region Constructors

        /// <summary>
        /// CookedRabbit RabbitPerformanceService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitSerializeService(RabbitSeasoning rabbitSeasoning, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = Factories.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }

        /// <summary>
        /// CookedRabbit RabbitPerformanceService constructor.  Allows for the sharing of a channel pool. If channel is not initialized, it will automatically initialize in here.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <param name="logger"></param>
        public RabbitSerializeService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = rcp;

            if (!_rcp.IsInitialized)
            { _rcp.Initialize(rabbitSeasoning).GetAwaiter().GetResult(); }
        }

        /// <summary>
        /// CookedRabbit RabbitPerformanceService constructor. Allows for the sharing of a channel pool and connection pool.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rchanp"></param>
        /// <param name="rconp"></param>
        /// <param name="logger"></param>
        public RabbitSerializeService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rchanp, IRabbitConnectionPool rconp, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;

            rchanp.SetConnectionPoolAsync(rabbitSeasoning, rconp).GetAwaiter().GetResult();

            _rcp = rchanp;
        }

        #endregion

        #region Serialize Section

        /// <summary>
        /// Serialize and publish a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> SerializeAndPublishAsync<T>(T message, Envelope envelope)
        {
            envelope.MessageBody = await SerializeAsync(message, _seasoning.SerializationMethod);

            if (_seasoning.CompressionEnabled)
            { envelope.MessageBody = await CompressAsync(envelope.MessageBody, _seasoning.CompressionMethod); }

            return await PublishAsync(envelope);
        }

        /// <summary>
        /// Serialize and publish messages asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages">The messages that will go in letters.</param>
        /// <param name="envelopeTemplate">Envelope to use as a template for creating letters.</param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        public async Task<List<int>> SerializeAndPublishManyAsync<T>(List<T> messages, Envelope envelopeTemplate)
        {
            var letters = new List<Envelope>();

            for (int i = 0; i < messages.Count; i++)
            {
                var envelope = envelopeTemplate.Clone();
                envelope.MessageBody = await SerializeAsync(messages[i], _seasoning.SerializationMethod);

                if (_seasoning.CompressionEnabled)
                { envelope.MessageBody = await CompressAsync(envelope.MessageBody, _seasoning.CompressionMethod); }

                letters.Add(envelope);
            }

            return await PublishManyAsync(letters);
        }

        /// <summary>
        /// Compress and Publish a message asynchronously (compression needs to be enabled). Envelope body cannot be null.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> CompressAndPublishAsync(Envelope envelope)
        {
            if (envelope.MessageBody is null) throw new ArgumentNullException(nameof(envelope.MessageBody));

            if (_seasoning.CompressionEnabled)
            { envelope.MessageBody = await CompressAsync(envelope.MessageBody, _seasoning.CompressionMethod); }

            return await PublishAsync(envelope);
        }

        /// <summary>
        /// Get and deserialize a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <returns>An object of type T.</returns>
        public async Task<T> GetAndDeserializeAsync<T>(string queueName)
        {
            var result = (await GetAsync(queueName));
            if (result is null) { return default; }

            var data = result.Body;

            if (_seasoning.CompressionEnabled)
            { data = await DecompressAsync(data, _seasoning.CompressionMethod); }

            return await DeserializeAsync<T>(data, _seasoning.SerializationMethod);
        }

        /// <summary>
        /// Get many messages and deserialize the messages asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="batchCount"></param>
        /// <returns>An object of type T.</returns>
        public async Task<List<T>> GetAndDeserializeManyAsync<T>(string queueName, int batchCount)
        {
            var deserializedMessages = new List<T>();
            var results = await GetManyAsync(queueName, batchCount);

            foreach (var result in results)
            {
                if (result != null && result != default(BasicGetResult))
                {
                    var data = result.Body;

                    if (_seasoning.CompressionEnabled)
                    { data = await DecompressAsync(data, _seasoning.CompressionMethod); }

                    deserializedMessages.Add(await DeserializeAsync<T>(data, _seasoning.SerializationMethod));
                }
            }

            return deserializedMessages;
        }

        /// <summary>
        /// Gets and decompresses a message body/payload asynchronously.
        /// <para>Service needs to have CompressionEnabled:true and CompressionMethod:method needs to match what was placed in the queue.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task<byte[]> GetAndDecompressAsync(string queueName)
        {
            var result = (await GetAsync(queueName));
            if (result is null) { return null; }

            var data = result.Body;

            if (_seasoning.CompressionEnabled)
            { data = await DecompressAsync(data, _seasoning.CompressionMethod); }

            return data;
        }

        #endregion

        #region Publish And Get Section

        /// <summary>
        /// Publish messages asynchronously.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> PublishAsync(Envelope envelope)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                var messageProperties = channelPair.Channel.CreateBasicProperties();
                messageProperties.ContentEncoding = envelope.ContentEncoding.Description();
                messageProperties.ContentType = envelope.MessageType;

                channelPair.Channel.BasicPublish(exchange: envelope.ExchangeName ?? string.Empty,
                        routingKey: envelope.RoutingKey,
                        envelope.Mandatory,
                        basicProperties: messageProperties,
                        body: envelope.MessageBody);

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
        /// Publishes many messages asynchronously.
        /// </summary>
        /// <param name="letters"></param>
        /// <returns>A List of the indices that failed to publish for calling service/methods to retry.</returns>
        public async Task<List<int>> PublishManyAsync(List<Envelope> letters)
        {
            var failures = new List<int>();
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);
            var rand = new Random();

            var messageProperties = channelPair.Channel.CreateBasicProperties();

            for(int i = 0; i < letters.Count; i++)
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
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                {
                    failures.Add(i);
                    await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                }
                catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
                {
                    failures.Add(i);
                    await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                }
                catch (Exception e)
                {
                    failures.Add(i);
                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return failures;
        }

        /// <summary>
        /// Get messages asynchronously.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>A <see cref="RabbitMQ.Client.BasicGetResult"/></returns>
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
            finally { _rcp.ReturnChannelToPool(channelPair); }

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
                    catch (Exception e) when (results.Count > 0)
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

        /// <summary>
        /// Gets all messages from a queue asynchronously. Stops on empty queue or on first error.
        /// </summary>
        /// <param name="queueName">The queue to get messages from.</param>
        /// <returns>A List of <see cref="RabbitMQ.Client.BasicGetResult"/>.</returns>
        public async Task<List<BasicGetResult>> GetAllAsync(string queueName)
        {
            var rand = new Random();
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
                catch (Exception e) when (results.Count > 0)
                {
                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    break; // Does not throw so you can use the results already found.
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                {
                    await HandleError(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                    else { break; }
                }
                catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
                {
                    await HandleError(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                    else { break; }
                }
                catch (Exception e)
                {
                    await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                    if (_seasoning.ThrowExceptions) { throw; }
                    else { break; }
                }

                if (_seasoning.ThrottleFastBodyLoops)
                { await Task.Delay(rand.Next(0, 2)); }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return results;
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
