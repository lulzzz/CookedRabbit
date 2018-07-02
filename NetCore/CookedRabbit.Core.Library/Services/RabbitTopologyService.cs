using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.LogStrings.GenericMessages;
using static CookedRabbit.Core.Library.Utilities.LogStrings.RabbitServiceMessages;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for creating Exchanges, Queues, and creating Bindings.
    /// </summary>
    public class RabbitTopologyService : IRabbitTopologyService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IRabbitChannelPool _rcp = null;
        private readonly RabbitTopologySeasoning _seasoning = null; // Used if for recovery later.

        /// <summary>
        /// CookedRabbit RabbitTopologyService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitTopologyService(RabbitTopologySeasoning rabbitSeasoning, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = RabbitChannelPool.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }

        #region Queue & Maintenance Section

        /// <summary>
        /// Create a queue asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="durable"></param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> QueueDeclareAsync(string queueName, bool durable = true, bool exclusive = false,
                                                  bool autoDelete = false, IDictionary<string, object> args = null)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.QueueDeclare(queue: queueName,
                    durable: durable,
                    exclusive: exclusive,
                    autoDelete: autoDelete,
                    arguments: args);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

        /// <summary>
        /// Delete a queue asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="onlyIfUnused"></param>
        /// <param name="onlyIfEmpty"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> QueueDeleteAsync(string queueName, bool onlyIfUnused = false, bool onlyIfEmpty = false)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.QueueDelete(queue: queueName,
                    ifUnused: onlyIfUnused,
                    ifEmpty: onlyIfEmpty);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        /// <summary>
        /// Bind a queue to exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> QueueBindToExchangeAsync(string queueName, string exchangeName, string routingKey = "",
                                                         IDictionary<string, object> args = null)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.QueueBind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: routingKey,
                    arguments: args);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        /// <summary>
        /// Unbind a queue from Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> QueueUnbindFromExchangeAsync(string queueName, string exchangeName, string routingKey = "",
                                                             IDictionary<string, object> args = null)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.QueueUnbind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: routingKey,
                    arguments: args);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        #endregion

        #region Exchange & Maintenance Section

        /// <summary>
        /// Create an Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> ExchangeDeclareAsync(string exchangeName, string exchangeType, bool durable = true,
                                                     bool autoDelete = false, IDictionary<string, object> args = null)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.ExchangeDeclare(exchange: exchangeName,
                    type: exchangeType,
                    durable: durable,
                    autoDelete: autoDelete,
                    arguments: args);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        /// <summary>
        /// Delete an Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="onlyIfUnused"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> ExchangeDeleteAsync(string exchangeName, bool onlyIfUnused = false)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.ExchangeDelete(exchange: exchangeName,
                    ifUnused: onlyIfUnused);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        /// <summary>
        /// Bind an Exchange to another Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="childExchangeName"></param>
        /// <param name="parentExchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> ExchangeBindToExchangeAsync(string childExchangeName, string parentExchangeName, string routingKey = "",
                                                            IDictionary<string, object> args = null)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.ExchangeBind(destination: childExchangeName,
                    source: parentExchangeName,
                    routingKey: routingKey,
                    arguments: args);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        /// <summary>
        /// Unbind an Exchange from another Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="childExchangeName"></param>
        /// <param name="parentExchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        public async Task<bool> ExchangeUnbindToExchangeAsync(string childExchangeName, string parentExchangeName, string routingKey = "",
                                                              IDictionary<string, object> args = null)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.ExchangeUnbind(destination: childExchangeName,
                    source: parentExchangeName,
                    routingKey: routingKey,
                    arguments: args);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                await ReportErrors(ace, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies)
            {
                await ReportErrors(rabbies, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            catch (Exception e)
            {
                await ReportErrors(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        #endregion

        #region Error Handling Section

        private async Task ReportErrors(Exception e, ulong channelId, params object[] args)
        {
            _rcp.FlagDeadChannel(channelId);
            var errorMessage = string.Empty;

            switch (e)
            {
                case RabbitMQ.Client.Exceptions.AlreadyClosedException ace:
                    errorMessage = ClosedChannelMessage;
                    break;
                case RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies:
                    errorMessage = RabbitExceptionMessage;
                    break;
                case Exception ex:
                    errorMessage = UnknownExceptionMessage;
                    break;
                default: break;
            }

            if (_seasoning.WriteErrorsToILogger)
            {
                if (_logger is null)
                { await Console.Out.WriteLineAsync($"{NullLoggerMessage} Exception:{e.Message}"); }
                else
                { _logger.LogError(e, errorMessage, args); }
            }

            if (_seasoning.WriteErrorsToConsole)
            { await Console.Out.WriteLineAsync(e.Message); }
        }

        #endregion

        #region Dispose

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
