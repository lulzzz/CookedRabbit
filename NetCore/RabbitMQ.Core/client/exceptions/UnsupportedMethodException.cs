using System;

namespace RabbitMQ.Client.Exceptions
{
    /// <summary>
    /// Thrown when the model receives an RPC request it cannot satisfy.
    /// </summary>
    public class UnsupportedMethodException : NotSupportedException
    {
        public UnsupportedMethodException(string methodName)
        {
            MethodName = methodName;
        }

        ///<summary>The name of the RPC request that could not be sent.</summary>
        public string MethodName { get; private set; }
    }
}
