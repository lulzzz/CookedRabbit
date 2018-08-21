using System;
using RabbitMQ.Client.Framing;

namespace RabbitMQ.Client.Impl
{
    /// <summary> Thrown when our peer sends a frame that contains
    /// illegal values for one or more fields. </summary>
    public class SyntaxError : HardProtocolException
    {
        public SyntaxError(string message) : base(message)
        {
        }

        public override ushort ReplyCode
        {
            get { return Constants.SyntaxError; }
        }
    }
}
