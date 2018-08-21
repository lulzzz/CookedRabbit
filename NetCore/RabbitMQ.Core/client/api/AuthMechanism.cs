namespace RabbitMQ.Client
{
    /// <summary>
    /// A pluggable authentication mechanism.
    /// </summary>
    public interface AuthMechanism
    {
        /// <summary>
        /// Handle one round of challenge-response.
        /// </summary>
        byte[] handleChallenge(byte[] challenge, IConnectionFactory factory);
    }
}
