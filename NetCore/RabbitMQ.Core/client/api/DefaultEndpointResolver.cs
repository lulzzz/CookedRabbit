﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Client
{
    public class DefaultEndpointResolver : IEndpointResolver
    {
        private List<AmqpTcpEndpoint> endpoints;
        private Random rnd = new Random();

        public DefaultEndpointResolver(IEnumerable<AmqpTcpEndpoint> tcpEndpoints)
        {
            endpoints = tcpEndpoints.ToList();
        }

        public IEnumerable<AmqpTcpEndpoint> All()
        {
            return endpoints.OrderBy(item => rnd.Next());
        }
    }
}