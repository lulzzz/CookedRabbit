using System;

namespace RabbitMQ.Client
{
    /// <summary>
    /// Information about the reason why a particular model, session, or connection was destroyed.
    /// </summary>
    /// <remarks>
    /// The <see cref="ClassId"/> and <see cref="Initiator"/> properties should be used to determine the originator of the shutdown event.
    /// </remarks>
    public class ShutdownEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a <see cref="ShutdownEventArgs"/> with the given parameters and
        ///  0 for <see cref="ClassId"/> and <see cref="MethodId"/>.
        /// </summary>
        public ShutdownEventArgs(ShutdownInitiator initiator, ushort replyCode, string replyText, object cause = null)
            : this(initiator, replyCode, replyText, 0, 0, cause)
        {
        }

        /// <summary>
        /// Construct a <see cref="ShutdownEventArgs"/> with the given parameters.
        /// </summary>
        public ShutdownEventArgs(ShutdownInitiator initiator, ushort replyCode, string replyText,
            ushort classId, ushort methodId, object cause = null)
        {
            Initiator = initiator;
            ReplyCode = replyCode;
            ReplyText = replyText;
            ClassId = classId;
            MethodId = methodId;
            Cause = cause;
        }

        /// <summary>
        /// Object causing the shutdown, or null if none.
        /// </summary>
        public object Cause { get; private set; }

        /// <summary>
        /// AMQP content-class ID, or 0 if none.
        /// </summary>
        public ushort ClassId { get; private set; }

        /// <summary>
        /// Returns the source of the shutdown event: either the application, the library, or the remote peer.
        /// </summary>
        public ShutdownInitiator Initiator { get; private set; }

        /// <summary>
        /// AMQP method ID within a content-class, or 0 if none.
        /// </summary>
        public ushort MethodId { get; private set; }

        /// <summary>
        /// One of the standardised AMQP reason codes. See RabbitMQ.Client.Framing.*.Constants.
        /// </summary>
        public ushort ReplyCode { get; private set; }

        /// <summary>
        /// Informative human-readable reason text.
        /// </summary>
        public string ReplyText { get; private set; }

        /// <summary>
        /// Override ToString to be useful for debugging.
        /// </summary>
        public override string ToString()
        {
            return "AMQP close-reason, initiated by " + Initiator +
                   ", code=" + ReplyCode +
                   ", text=\"" + ReplyText + "\"" +
                   ", classId=" + ClassId +
                   ", methodId=" + MethodId +
                   ", cause=" + Cause;
        }
    }
}
