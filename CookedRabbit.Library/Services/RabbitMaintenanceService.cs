using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Utilities;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for doing on the fly of maintenance.
    /// </summary>
    public class RabbitMaintenanceService : RabbitBaseService, IRabbitMaintenanceService, IDisposable
    {
        #region Constructor Section

        /// <summary>
        /// CookedRabbit RabbitMaintenanceService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitMaintenanceService(RabbitSeasoning rabbitSeasoning, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = Factories.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }

        /// <summary>
        /// CookedRabbit RabbitMaintenanceService constructor.  Allows for the sharing of a channel pool. If channel is not initialized, it will automatically initialize in here.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <param name="logger"></param>
        public RabbitMaintenanceService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = rcp;

            if (!_rcp.IsInitialized)
            { _rcp.Initialize(rabbitSeasoning).GetAwaiter().GetResult(); }
        }

        /// <summary>
        /// CookedRabbit RabbitTopologyService constructor. Allows for the sharing of a channel pool and connection pool.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rchanp"></param>
        /// <param name="rconp"></param>
        /// <param name="logger"></param>
        public RabbitMaintenanceService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rchanp, IRabbitConnectionPool rconp, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;

            rchanp.SetConnectionPoolAsync(rabbitSeasoning, rconp).GetAwaiter().GetResult();

            _rcp = rchanp;
        }

        #endregion

        #region Maintenance Section

        /// <summary>
        /// Empty/purge the queue.
        /// </summary>
        /// <param name="queueName">The queue to remove from.</param>
        /// <param name="deleteQueueAfter">Indicate if you want to delete the queue after purge.</param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> PurgeQueueAsync(string queueName, bool deleteQueueAfter = false)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.QueuePurge(queueName);

                if (deleteQueueAfter)
                {
                    channelPair.Channel.QueueDelete(queueName, false, false);
                }

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
        /// Transfers one message from one queue to another queue.
        /// </summary>
        /// <param name="originQueueName">The queue to remove from.</param>
        /// <param name="targetQueueName">The destination queue.</param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> TransferMessageAsync(string originQueueName, string targetQueueName)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                var result = channelPair.Channel.BasicGet(originQueueName, true);

                if (result != null && result.Body != null)
                { channelPair.Channel.BasicPublish(string.Empty, targetQueueName, false, null, result.Body); }

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
        /// Transfers a number of messages from one queue to another queue.
        /// </summary>
        /// <param name="originQueueName">The queue to remove from.</param>
        /// <param name="targetQueueName">The destination queue.</param>
        /// <param name="count">Number of messages to transfer</param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> TransferMessagesAsync(string originQueueName, string targetQueueName, ushort count)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                for (ushort i = 0; i < count; i++)
                {
                    var result = channelPair.Channel.BasicGet(originQueueName, true);

                    if (result != null && result.Body != null)
                    { channelPair.Channel.BasicPublish(string.Empty, targetQueueName, false, null, result.Body); }
                }

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
        /// Transfers all messages from one queue to another queue. Stops on the first null result (when queue is empty).
        /// </summary>
        /// <param name="originQueueName">The queue to remove from.</param>
        /// <param name="targetQueueName">The destination queue.</param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> TransferAllMessagesAsync(string originQueueName, string targetQueueName)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                BasicGetResult result = null;

                while (result != null)
                {
                    result = channelPair.Channel.BasicGet(originQueueName, true);

                    if (result != null && result.Body != null)
                    { channelPair.Channel.BasicPublish(string.Empty, targetQueueName, false, null, result.Body); }
                }

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
