using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Utilities;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static CookedRabbit.Library.Utilities.ApiHelpers;
using static CookedRabbit.Library.Utilities.Enums;
using static CookedRabbit.Library.Utilities.RandomData;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for doing on the fly maintenance. Inherits from RabbitTopologyService.
    /// </summary>
    public class RabbitMaintenanceService : RabbitTopologyService, IRabbitMaintenanceService, IDisposable
    {
        private readonly Task _pingPong = Task.CompletedTask;
        private readonly HttpClient _httpClient = null;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #region Constructor Section

        /// <summary>
        /// CookedRabbit RabbitMaintenanceService constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitMaintenanceService(RabbitSeasoning rabbitSeasoning, ILogger logger = null) : base(rabbitSeasoning, logger)
        {
            Guard.AgainstNull(rabbitSeasoning, nameof(rabbitSeasoning));
            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = Factories.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();

            if (_seasoning.MaintenanceSettings.EnablePingPong)
            { _pingPong = PingPongAsync($"{_seasoning.MaintenanceSettings.PingPongQueueName}.{RandomString(5, 10)}", _cancellationTokenSource.Token); }

            if (_seasoning.MaintenanceSettings.ApiSettings.RabbitApiAccessEnabled)
            {
                var credentials = new NetworkCredential(
                    _seasoning.MaintenanceSettings.ApiSettings.RabbitApiUserName,
                    _seasoning.MaintenanceSettings.ApiSettings.RabbitApiUserPassword);

                _httpClient = CreateHttpClient(credentials, TimeSpan.FromSeconds(_seasoning.MaintenanceSettings.ApiSettings.RabbitApiTimeout));
            }
        }

        /// <summary>
        /// CookedRabbit RabbitMaintenanceService constructor. Allows for the sharing of a channel pool. If channel pool is not initialized, it will automatically initialize in here.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <param name="logger"></param>
        public RabbitMaintenanceService(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null) : base(rabbitSeasoning, rcp, logger)
        {
            Guard.AgainstNull(rabbitSeasoning, nameof(rabbitSeasoning));
            Guard.AgainstNull(rcp, nameof(rcp));

            _logger = logger;
            _seasoning = rabbitSeasoning;
            _rcp = rcp;

            if (!_rcp.IsInitialized)
            { _rcp.Initialize(rabbitSeasoning).GetAwaiter().GetResult(); }

            if (_seasoning.MaintenanceSettings.EnablePingPong)
            { _pingPong = PingPongAsync($"{_seasoning.MaintenanceSettings.PingPongQueueName}.{RandomString(5, 10)}", _cancellationTokenSource.Token); }

            if (_seasoning.MaintenanceSettings.ApiSettings.RabbitApiAccessEnabled)
            {
                var credentials = new NetworkCredential(
                    _seasoning.MaintenanceSettings.ApiSettings.RabbitApiUserName,
                    _seasoning.MaintenanceSettings.ApiSettings.RabbitApiUserPassword);

                _httpClient = CreateHttpClient(credentials, TimeSpan.FromSeconds(_seasoning.MaintenanceSettings.ApiSettings.RabbitApiTimeout));
            }
        }

        /// <summary>
        /// CookedRabbit RabbitTopologyService constructor. Allows for the sharing of a channel pool and connection pool.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rchanp"></param>
        /// <param name="rconp"></param>
        /// <param name="logger"></param>
        public RabbitMaintenanceService(RabbitSeasoning rabbitSeasoning,
            IRabbitChannelPool rchanp,
            IRabbitConnectionPool rconp,
            ILogger logger = null) : base(rabbitSeasoning, rchanp, rconp, logger)
        {
            Guard.AgainstNull(rabbitSeasoning, nameof(rabbitSeasoning));
            Guard.AgainstNull(rchanp, nameof(rchanp));
            Guard.AgainstNull(rconp, nameof(rconp));

            _logger = logger;
            _seasoning = rabbitSeasoning;

            rchanp.SetConnectionPoolAsync(rabbitSeasoning, rconp).GetAwaiter().GetResult();
            _rcp = rchanp;

            if (_seasoning.MaintenanceSettings.EnablePingPong)
            { _pingPong = PingPongAsync($"{_seasoning.MaintenanceSettings.PingPongQueueName}.{RandomString(5, 10)}", _cancellationTokenSource.Token); }

            if (_seasoning.MaintenanceSettings.ApiSettings.RabbitApiAccessEnabled)
            {
                var credentials = new NetworkCredential(
                    _seasoning.MaintenanceSettings.ApiSettings.RabbitApiUserName,
                    _seasoning.MaintenanceSettings.ApiSettings.RabbitApiUserPassword);

                _httpClient = CreateHttpClient(credentials, TimeSpan.FromSeconds(_seasoning.MaintenanceSettings.ApiSettings.RabbitApiTimeout));
            }
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
            Guard.AgainstNullOrEmpty(queueName, nameof(queueName));

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
            Guard.AgainstNullOrEmpty(originQueueName, nameof(originQueueName));
            Guard.AgainstNullOrEmpty(targetQueueName, nameof(targetQueueName));

            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                var result = channelPair.Channel.BasicGet(originQueueName, true);

                if (result != null && result.Body != null)
                { channelPair.Channel.BasicPublish(string.Empty, targetQueueName, false, null, result.Body); }

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
        /// Transfers a number of messages from one queue to another queue.
        /// </summary>
        /// <param name="originQueueName">The queue to remove from.</param>
        /// <param name="targetQueueName">The destination queue.</param>
        /// <param name="count">Number of messages to transfer</param>
        /// <returns>A bool indicating success or failure.</returns>
        public async Task<bool> TransferMessagesAsync(string originQueueName, string targetQueueName, ushort count)
        {
            Guard.AgainstNullOrEmpty(originQueueName, nameof(originQueueName));
            Guard.AgainstNullOrEmpty(targetQueueName, nameof(targetQueueName));

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
            Guard.AgainstNullOrEmpty(originQueueName, nameof(originQueueName));
            Guard.AgainstNullOrEmpty(targetQueueName, nameof(targetQueueName));

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
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });

                if (_seasoning.ThrowExceptions) { throw; }
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        #endregion

        #region PingPong Section

        private readonly byte[] _testPayload = new byte[] { 0x00b, 0x0Fb, 0x00b, 0x0Fb };
        private readonly long?[] _times = new long?[_timesLength];
        private const ushort _timesLength = 10;
        private int _currentIndex = 0;

        private readonly object _timesLock = new object();
        private readonly int _timeout = 5; // seconds
        private readonly int _delayForServerSideRouting = 50;

        private async Task<bool> PublishTestMessageAsync(string queueName)
        {
            var success = false;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                channelPair.Channel.BasicPublish(exchange: string.Empty,
                    routingKey: queueName,
                    body: _testPayload);

                success = true;
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return success;
        }

        private async Task<byte[]> GetTestMessageAsync(string queueName)
        {
            byte[] body = null;
            var channelPair = await _rcp.GetPooledChannelPairAsync().ConfigureAwait(false);

            try
            {
                var result = channelPair.Channel.BasicGet(queueName, true);

                if (result != null && result.Body != null)
                { body = result.Body; }
            }
            catch (Exception e)
            {
                await HandleError(e, channelPair.ChannelId, new { channelPair.ChannelId });
            }
            finally { _rcp.ReturnChannelToPool(channelPair); }

            return body;
        }

        private async Task PingPongAsync(string queueName, CancellationToken token)
        {
            var queueDeclared = false;
            Stopwatch sw = null;

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(_seasoning.MaintenanceSettings.PingPongTime);
                sw = Stopwatch.StartNew();

                if (!queueDeclared) { queueDeclared = await QueueDeclareAsync(queueName); }
                var testSuccess = await PublishTestMessageAsync(queueName);

                sw.Stop();
                await Task.Delay(_delayForServerSideRouting); // Allow time for server side Routing.
                sw.Start();

                if (testSuccess)
                {
                    var payload = await GetTestMessageAsync(queueName);
                    sw.Stop();

                    if (payload != null)
                    {
                        // Record successful Result
                        if (Monitor.TryEnter(_timesLock, TimeSpan.FromSeconds(_timeout)))
                        {
                            _times[_currentIndex] = sw.ElapsedMilliseconds;
                            Monitor.Exit(_timesLock);
                        }
                    }
                    else
                    {
                        // Indicate a failure (null)
                        if (Monitor.TryEnter(_timesLock, TimeSpan.FromSeconds(_timeout)))
                        {
                            _times[_currentIndex] = null;
                            Monitor.Exit(_timesLock);
                        }
                    }

                    // Adjust index.
                    if (_currentIndex == _timesLength - 1) { _currentIndex = 0; }
                    else { _currentIndex++; }
                }
            }
        }

        /// <summary>
        /// Get the average response times from the last 5 ping pong results and the number of misses.
        /// </summary>
        /// <returns>A ValueTuple(int, double). It indicates the number of failed ping pongs (misses) and the average response time.</returns>
        public (int Misses, double AverageResponseTime) GetAverageResponseTimes()
        {
            if (!_seasoning.MaintenanceSettings.EnablePingPong) throw new Exception("Can't get average ping pong results if it is not enabled.");

            (int Misses, double AverageResponseTime) result = (0, 0.0d);

            if (Monitor.TryEnter(_timesLock, TimeSpan.FromSeconds(_timeout)))
            {
                for (int i = 0; i < _times.Length; i++)
                {
                    if (_times[i] is null) { result.Misses++; }
                    else { result.AverageResponseTime += (double)_times[i]; }
                }

                if (_times.Length != result.Misses)
                {
                    result.AverageResponseTime = result.AverageResponseTime / (_times.Length - result.Misses);
                }

                Monitor.Exit(_timesLock);
            }

            return result;
        }

        #endregion

        #region Api Section

        private string _createApiBasePath = string.Empty;

        /// <summary>
        /// Performs API Get calls based on the API target.
        /// </summary>
        /// <typeparam name="TResult">Internal model to be mapped to the specific API call result.</typeparam>
        /// <param name="rabbitApiTarget">Sepcific API endpoint.</param>
        /// <returns>TResult deserialized from JSON.</returns>
        public async Task<TResult> Api_GetAsync<TResult>(RabbitApiTarget rabbitApiTarget)
        {
            Guard.AgainstNullOrEmpty(_seasoning.MaintenanceSettings.ApiSettings.RabbitApiHostName, nameof(_seasoning.MaintenanceSettings.ApiSettings.RabbitApiHostName));

            if (string.IsNullOrEmpty(_createApiBasePath))
            {
                _createApiBasePath = CreateApiBasePath(_seasoning.MaintenanceSettings.ApiSettings.UseSsl,
                _seasoning.MaintenanceSettings.ApiSettings.RabbitApiHostName,
                _seasoning.MaintenanceSettings.ApiSettings.RabbitApiPort);
            }

            var path = _createApiBasePath;
            var request = CreateRequest(path, rabbitApiTarget.Description(), HttpMethod.Get);
            var response = await _httpClient.SendAsync(request);

            string content = null;
            if (response.IsSuccessStatusCode)
            { content = await response.Content.ReadAsStringAsync(); }

            TResult result = default;
            if (content != null)
            { result = Utf8Json.JsonSerializer.Deserialize<TResult>(content); }

            return result;
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
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _rcp.Shutdown();
                }
                _disposedValue = true;
            }
        }

        void IDisposable.Dispose() { Dispose(true); }

        #endregion
    }
}
