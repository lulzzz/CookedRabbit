using CookedRabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookedRabbit.Bus
{
    public class RabbitBus : IRabbitBus
    {
        private readonly IRabbitService _rabbitService;

        public RabbitBus(IRabbitService rabbitService)
        {
            _rabbitService = rabbitService;
        }
    }
}
