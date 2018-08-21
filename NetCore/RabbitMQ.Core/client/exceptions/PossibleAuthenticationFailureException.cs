using System;

namespace RabbitMQ.Client.Exceptions
{
    /// <summary> Thrown when the likely cause is  an
    /// authentication failure. </summary>
    public class PossibleAuthenticationFailureException : RabbitMQClientException
    {
        public PossibleAuthenticationFailureException(String msg, Exception inner) : base(msg, inner)
        {
        }
        public PossibleAuthenticationFailureException(String msg) : base(msg)
        {
        }
    }
}
