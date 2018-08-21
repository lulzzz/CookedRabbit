namespace RabbitMQ.Client.Impl
{
    ///<summary>Subclass of ProtocolException representing problems
    ///requiring a connection.close.</summary>
    public abstract class HardProtocolException : ProtocolException
    {
        protected HardProtocolException(string message) : base(message)
        {
        }
    }
}
