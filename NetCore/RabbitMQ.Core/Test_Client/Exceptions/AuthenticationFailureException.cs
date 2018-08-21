using System;

namespace RabbitMQ.Client.Exceptions
{
    /// <summary> Thrown when the cause is  an
    /// authentication failure. </summary>
    public class AuthenticationFailureException : PossibleAuthenticationFailureException
    {
        public AuthenticationFailureException(string msg) : base(msg)
        {
        }
    }
}
