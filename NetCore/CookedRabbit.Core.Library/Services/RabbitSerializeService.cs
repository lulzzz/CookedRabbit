using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Utilities;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.Compression;
using static CookedRabbit.Core.Library.Utilities.Serialization;

namespace CookedRabbit.Core.Library.Services
{
    /// <summary>
    /// CookedRabbit service for opinionated performance serialize and deserialize options. Inherits from RabbitDeliveryService.
    /// </summary>
    public class RabbitSerializeService : RabbitDeliveryService, IRabbitSerializeService, IRabbitDeliveryService, IDisposable
    {
        #region Constructors

        /// <summary>
        /// CookedRabbit RabbitSerializeService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitSerializeService(RabbitSeasoning rabbitSeasoning, ILogger logger = null) : base(rabbitSeasoning, logger)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = Factories.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }

        /// <summary>
        /// CookedRabbit RabbitSerializeService constructor. Allows for the sharing of a channel pool. If channel pool is not initialized, it will automatically initialize in here.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <param name="logger"></param>
        public RabbitSerializeService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null) : base(rabbitSeasoning, rcp, logger)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = rcp;

            if (!_rcp.IsInitialized)
            { _rcp.Initialize(rabbitSeasoning).GetAwaiter().GetResult(); }
        }

        /// <summary>
        /// CookedRabbit RabbitSerializeService constructor. Allows for the sharing of a channel pool and connection pool.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rchanp"></param>
        /// <param name="rconp"></param>
        /// <param name="logger"></param>
        public RabbitSerializeService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rchanp, IRabbitConnectionPool rconp, ILogger logger = null) : base(rabbitSeasoning, rchanp, rconp, logger)
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
            envelope.MessageBody = await SerializeAsync(message, _seasoning.SerializeSettings.SerializationMethod);

            if (_seasoning.SerializeSettings.CompressionEnabled)
            { envelope.MessageBody = await CompressAsync(envelope.MessageBody, _seasoning.SerializeSettings.CompressionMethod); }

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
                envelope.MessageBody = await SerializeAsync(messages[i], _seasoning.SerializeSettings.SerializationMethod);

                if (_seasoning.SerializeSettings.CompressionEnabled)
                { envelope.MessageBody = await CompressAsync(envelope.MessageBody, _seasoning.SerializeSettings.CompressionMethod); }

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

            if (_seasoning.SerializeSettings.CompressionEnabled)
            { envelope.MessageBody = await CompressAsync(envelope.MessageBody, _seasoning.SerializeSettings.CompressionMethod); }

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

            if (_seasoning.SerializeSettings.CompressionEnabled)
            { data = await DecompressAsync(data, _seasoning.SerializeSettings.CompressionMethod); }

            return await DeserializeAsync<T>(data, _seasoning.SerializeSettings.SerializationMethod);
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

                    if (_seasoning.SerializeSettings.CompressionEnabled)
                    { data = await DecompressAsync(data, _seasoning.SerializeSettings.CompressionMethod); }

                    deserializedMessages.Add(await DeserializeAsync<T>(data, _seasoning.SerializeSettings.SerializationMethod));
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

            if (_seasoning.SerializeSettings.CompressionEnabled)
            { data = await DecompressAsync(data, _seasoning.SerializeSettings.CompressionMethod); }

            return data;
        }

        #endregion

        #region Dispose Section

        private bool _disposedValue = false;

        /// <summary>
        /// RabbitService dispose method.
        /// </summary>
        /// <param name="disposing"></param>
        public override void Dispose(bool disposing)
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
