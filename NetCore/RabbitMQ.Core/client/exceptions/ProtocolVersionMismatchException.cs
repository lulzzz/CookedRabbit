namespace RabbitMQ.Client.Exceptions
{
    ///<summary>Thrown to indicate that the peer does not support the
    ///wire protocol version we requested immediately after opening
    ///the TCP socket.</summary>
    public class ProtocolVersionMismatchException : ProtocolViolationException
    {
        ///<summary>Fills the new instance's properties with the values passed in.</summary>
        public ProtocolVersionMismatchException(int clientMajor,
            int clientMinor,
            int serverMajor,
            int serverMinor)
            : base($"AMQP server protocol negotiation failure: server version {PositiveOrUnknown(serverMajor)}-{PositiveOrUnknown(serverMinor)}, client version {PositiveOrUnknown(clientMajor)}-{PositiveOrUnknown(clientMinor)}")
        {
            ClientMajor = clientMajor;
            ClientMinor = clientMinor;
            ServerMajor = serverMajor;
            ServerMinor = serverMinor;
        }

        ///<summary>The client's AMQP specification major version.</summary>
        public int ClientMajor { get; private set; }

        ///<summary>The client's AMQP specification minor version.</summary>
        public int ClientMinor { get; private set; }

        ///<summary>The peer's AMQP specification major version.</summary>
        public int ServerMajor { get; private set; }

        ///<summary>The peer's AMQP specification minor version.</summary>
        public int ServerMinor { get; private set; }

        private static string PositiveOrUnknown(int version)
        {
            return version >= 0 ? version.ToString() : "unknown";
        }
    }
}
