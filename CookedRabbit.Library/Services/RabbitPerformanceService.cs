using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Utilities;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;
using static CookedRabbit.Library.Utilities.Compression;
using static CookedRabbit.Library.Utilities.Enums;
using static CookedRabbit.Library.Utilities.Serialization;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for opinionated performance delivery and receive options.
    /// </summary>
    public class RabbitPerformanceService : RabbitBaseService, IRabbitPerformanceService, IDisposable
    {
        /// <summary>
        /// CookedRabbit RabbitPerformanceService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitPerformanceService(RabbitSeasoning rabbitSeasoning, ILogger logger = null)
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
        public RabbitPerformanceService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null)
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
        public RabbitPerformanceService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rchanp, IRabbitConnectionPool rconp, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;

            rchanp.SetConnectionPoolAsync(rabbitSeasoning, rconp).GetAwaiter().GetResult();

            _rcp = rchanp;
        }

        /// <summary>
        /// Serialize and publish a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="envelope"></param>
        /// <param name="serializationMethod"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> SerializeAndPublishAsync<T>(T message, Envelope envelope,
            SerializationMethod serializationMethod = SerializationMethod.Utf8Json)
        {
            Task<byte[]> compressionTask = null;
            envelope.MessageBody = await SerializeAsync(message, serializationMethod);

            if (_seasoning.CompressionEnabled)
            { compressionTask = CompressAsync(envelope.MessageBody, _seasoning.CompressionMethod); }

            if (compressionTask != null)
            {
                await compressionTask;
                envelope.MessageBody = compressionTask.Result;
            }

            return await PublishAsync(envelope);
        }

        /// <summary>
        /// Get and deserialize a message asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="serializationMethod"></param>
        /// <returns>A an object of type T.</returns>
        public async Task<T> GetAndDeserializeAsync<T>(string queueName,
            SerializationMethod serializationMethod = SerializationMethod.Utf8Json)
        {
            Task<byte[]> decompressionTask = null;
            var result = await GetAsync(queueName);
            var data = result.Body;

            if (_seasoning.CompressionEnabled)
            { decompressionTask = DecompressAsync(data, _seasoning.CompressionMethod); }

            if (decompressionTask != null)
            {
                await decompressionTask;
                data = decompressionTask.Result;
            }

            return await DeserializeAsync<T>(data, serializationMethod);
        }

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
