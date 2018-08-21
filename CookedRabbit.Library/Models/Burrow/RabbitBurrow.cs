using CookedRabbit.Library.Pools;
using CookedRabbit.Library.Services;
using CookedRabbit.Library.Utilities;
using Microsoft.Extensions.Logging;
using System;

namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// A RabbitBurrow contains all the pre-built paths for Rabbits to take.
    /// </summary>
    public class RabbitBurrow : IDisposable
    {
        private readonly RabbitSeasoning _seasoning;

        /// <summary>
        /// CookedRabbit RabbitSerializeService for Serilization, Compression, and Delivery methods.
        /// </summary>
        public RabbitSerializeService Transmission;
        /// <summary>
        /// CookedRabbit RabbitMaintenanceService for Queue/Exchange Management and Topology methods.
        /// </summary>
        public RabbitMaintenanceService Maintenance;

        /// <summary>
        /// CookedRabbit RabbitBurrow constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="logger"></param>
        public RabbitBurrow(RabbitSeasoning rabbitSeasoning, ILogger logger = null)
        {
            _seasoning = rabbitSeasoning;

            var channelPool = Factories.CreateRabbitChannelPoolAsync(_seasoning).GetAwaiter().GetResult();

            Transmission = new RabbitSerializeService(_seasoning, channelPool, logger);
            Maintenance = new RabbitMaintenanceService(_seasoning, channelPool, logger);
        }

        /// <summary>
        /// CookedRabbit RabbitBurrow constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <param name="logger"></param>
        public RabbitBurrow(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rcp, ILogger logger = null)
        {
            _seasoning = rabbitSeasoning;

            Transmission = new RabbitSerializeService(_seasoning, rcp, logger);
            Maintenance = new RabbitMaintenanceService(_seasoning, rcp, logger);
        }

        /// <summary>
        /// CookedRabbit RabbitBurrow constructor.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rchanp"></param>
        /// <param name="rconp"></param>
        /// <param name="logger"></param>
        public RabbitBurrow(RabbitSeasoning rabbitSeasoning, IRabbitChannelPool rchanp, IRabbitConnectionPool rconp, ILogger logger = null)
        {
            _seasoning = rabbitSeasoning;

            Transmission = new RabbitSerializeService(_seasoning, rchanp, rconp, logger);
            Maintenance = new RabbitMaintenanceService(_seasoning, rchanp, rconp, logger);
        }

        #region Dispose Section

        private bool _disposedValue = false;

        /// <summary>
        /// RabbitBurrow dispose method.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Transmission.Dispose();
                    Maintenance.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// RabbitBurrow dispose method.
        /// </summary>
        public void Dispose()
        { Dispose(true); }

        #endregion
    }
}
