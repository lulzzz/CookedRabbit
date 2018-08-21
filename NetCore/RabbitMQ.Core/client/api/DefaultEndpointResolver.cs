using System;
using System.Linq;
using System.Collections.Generic;

namespace RabbitMQ.Client
{
    public class DefaultEndpointResolver : IEndpointResolver
    {
        private List<AmqpTcpEndpoint> endpoints;
        private Random rnd = new Random();

        public DefaultEndpointResolver (IEnumerable<AmqpTcpEndpoint> tcpEndpoints)
        {
           this.endpoints = tcpEndpoints.ToList();
        }

        public IEnumerable<AmqpTcpEndpoint> All()
        {
            return endpoints.OrderBy(item => rnd.Next());
        }
    }
}