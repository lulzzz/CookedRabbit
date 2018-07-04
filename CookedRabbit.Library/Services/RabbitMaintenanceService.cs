using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static CookedRabbit.Library.Utilities.LogStrings.GenericMessages;
using static CookedRabbit.Library.Utilities.LogStrings.RabbitServiceMessages;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for doing on the fly of maintenance.
    /// </summary>
    public class RabbitMaintenanceService : IRabbitMaintenanceService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IRabbitChannelPool _rcp = null;
        private readonly RabbitSeasoning _seasoning = null; // Used for recovery later.

        /// <summary>
        /// CookedRabbit RabbitMaintenanceService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitMaintenanceService(RabbitSeasoning rabbitSeasoning, ILogger logger = null)
        {
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = RabbitChannelPool.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Empty the queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="deleteQueueAfter"></param>
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
        /// Transfers one message from one queue to another queue.
        /// </summary>
        /// <param name="originQueueName"></param>
        /// <param name="targetQueueName"></param>
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
        /// Transfers all messages from one queue to another queue. Stops when on the first null result.
        /// </summary>
        /// <param name="originQueueName"></param>
        /// <param name="targetQueueName"></param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> TransferAllMessageAsync(string originQueueName, string targetQueueName)
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

        #region Error Handling Section

        private async Task ReportErrors(Exception e, ulong channelId, params object[] args)
        {
            _rcp.FlagDeadChannel(channelId);
            var errorMessage = string.Empty;

            switch (e)
            {
                case RabbitMQ.Client.Exceptions.AlreadyClosedException ace:
                    e = ace.Demystify();
                    errorMessage = ClosedChannelMessage;
                    break;
                case RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies:
                    e = rabbies.Demystify();
                    errorMessage = RabbitExceptionMessage;
                    break;
                case Exception ex:
                    e = ex.Demystify();
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
