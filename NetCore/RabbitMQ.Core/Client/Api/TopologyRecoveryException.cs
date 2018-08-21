using RabbitMQ.Client.Exceptions;
using System;

namespace RabbitMQ.Client
{
    public class TopologyRecoveryException : RabbitMQClientException
    {
        public TopologyRecoveryException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
