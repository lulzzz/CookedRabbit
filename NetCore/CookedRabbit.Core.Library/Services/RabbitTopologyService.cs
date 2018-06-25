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
    public class RabbitTopologyService : IRabbitTopologyService
    {
        private readonly RabbitChannelPool _rcp = null;
        private readonly RabbitTopologySeasoning _originalRabbitSeasoning = null; // Used if for recovery later.

        public RabbitTopologyService(RabbitTopologySeasoning rabbitSeasoning)
        {
            _originalRabbitSeasoning = rabbitSeasoning;
            _rcp = RabbitChannelPool.CreateRabbitChannelPoolAsync(rabbitSeasoning).GetAwaiter().GetResult();
        }
    }
}
