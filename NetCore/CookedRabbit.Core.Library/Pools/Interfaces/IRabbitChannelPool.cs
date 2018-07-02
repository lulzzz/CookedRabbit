using RabbitMQ.Client;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Pools
{
    /// <summary>
    /// CookedRabbit RabbitChannelPool creates the connection pool and manages the channels.
    /// </summary>
    public interface IRabbitChannelPool
    {
        /// <summary>
        /// Creates a transient (untracked) RabbitMQ channel. Closing/Disposal is the responsibility of the calling service.
        /// </summary>
        /// <param name="enableAck"></param>
        /// <returns>Returns an IModel channel (RabbitMQ).</returns>
        Task<IModel> GetTransientChannelAsync(bool enableAck = false);

        /// <summary>
        /// Gets a pre-created (tracked) RabbitMQ channel. Must be returned by the calling the service!
        /// </summary>
        /// <returns>Returns a ValueTuple(ulong, IModel)</returns>
        Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPairAsync();

        /// <summary>
        /// Gets a pre-created (tracked) RabbitMQ channel that can acknowledge messages. Must be returned by the calling the service!
        /// </summary>
        /// <returns>Returns a ValueTuple(ulong, IModel)</returns>
        Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPairAckableAsync();


        /// <summary>
        /// Adds ChannelId to a ConcurrentBag. This indicates it will be removed on it's next turn for usage and a new channel will be created instead of using this one.
        /// </summary>
        /// <param name="deadChannelId"></param>
        void FlagDeadChannel(ulong deadChannelId);

        /// <summary>
        /// Called to return a channel (ackable) to its channel pool.
        /// </summary>
        /// <param name="ChannelPair"></param>
        /// <returns>Returns a bool indicating success or failure.</returns>
        bool ReturnChannelToPool((ulong ChannelId, IModel Channel) ChannelPair);

        /// <summary>
        /// Called to return a channel (ackable) to its channel pool.
        /// </summary>
        /// <param name="ChannelPair"></param>
        /// <returns>Returns a bool indicating success or failure.</returns>
        bool ReturnChannelToAckPool((ulong ChannelId, IModel Channel) ChannelPair);

        /// <summary>
        /// RabbitChannelPool shutdown method closes all channels, disposes each model, and refreshes the objects in memory.
        /// </summary>
        void Shutdown();
    }
}
