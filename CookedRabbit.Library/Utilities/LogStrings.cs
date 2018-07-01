namespace CookedRabbit.Library.Utilities.LogStrings
{
    /// <summary>
    /// Group of static strings for CookedRabbit to cut down on memory usage when logging errors.
    /// </summary>
    public static class GenericMessages
    {
        /// <summary>
        /// ILogger dependency is null message.
        /// </summary>
        public static string NullLoggerMessage = "CookedRabbit Logger Null: Unable to write error to logs as no ILogger was passed in.";
    }

    /// <summary>
    /// Group of static strings for RabbitService to cut down on memory usage when logging errors.
    /// </summary>
    public static class RabbitServiceMessages
    {
        /// <summary>
        /// Used a closed channel error message.
        /// </summary>
        public static string ClosedChannelMessage = "CookedRabbit used a closed channel. Channel flagged as dead and will be repaired.";

        /// <summary>
        /// Unknown RabbitMQ exception occurred on this channel error message.
        /// </summary>
        public static string RabbitExceptionMessage = "CookedRabbit used a channel that has rabbies. Channel flagged as dead and will be repaired.";

        /// <summary>
        /// Unknown exception occurred using this channel error message.
        /// </summary>
        public static string UnknownExceptionMessage = "CookedRabbit used a channel that generated an exception. Channel flagged as dead and will be repaired.";
    }
}
