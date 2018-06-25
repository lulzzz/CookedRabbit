using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    public class RabbitTopologyService : IRabbitTopologyService, IDisposable
    {
        private readonly RabbitChannelPool _rcp = null;
        private readonly RabbitTopologySeasoning _originalRabbitSeasoning = null; // Used if for recovery later.

        public RabbitTopologyService(RabbitTopologySeasoning rabbitSeasoning)
        {
            _originalRabbitSeasoning = rabbitSeasoning;
            _rcp = RabbitChannelPool.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }

        #region Queue & Maintenance Section

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

        #endregion

        #region Exchange & Maintenance Section

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

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
                _rcp.FlagDeadChannel(channelPair.ChannelId);
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            _rcp.ReturnChannelToPool(channelPair);

            return success;
        }

        #endregion

        #region Dispose

        private bool _disposedValue = false;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { _rcp.Dispose(true); }

                _disposedValue = true;
            }
        }

        void IDisposable.Dispose() { Dispose(true); }

        #endregion
    }
}
