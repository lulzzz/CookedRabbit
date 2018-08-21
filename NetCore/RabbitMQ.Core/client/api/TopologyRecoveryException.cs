using System;
using RabbitMQ.Client.Exceptions;

namespace RabbitMQ.Client
{
    public class TopologyRecoveryException : RabbitMQClientException
    {
        public TopologyRecoveryException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
