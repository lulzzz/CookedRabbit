namespace RabbitMQ.Client.Exceptions
{
    /// <summary>Thrown when the application tries to make use of a
    /// session or connection that has already been shut
    /// down.</summary>
    public class AlreadyClosedException : OperationInterruptedException
    {
        ///<summary>Construct an instance containing the given
        ///shutdown reason.</summary>
        public AlreadyClosedException(ShutdownEventArgs reason)
            : base(reason, "Already closed")
        {
        }
    }
}
