using System;
using System.IO;

namespace RabbitMQ.Client.Exceptions
{
    ///<summary>Thrown when no connection could be opened during a
    ///ConnectionFactory.CreateConnection attempt.</summary>
    public class BrokerUnreachableException : IOException
    {
        ///<summary>Construct a BrokerUnreachableException. The inner exception is
        ///an AggregateException holding the errors from multiple connection attempts.</summary>
        public BrokerUnreachableException(Exception Inner)
            : base("None of the specified endpoints were reachable", Inner)
        {
        }
    }
}
