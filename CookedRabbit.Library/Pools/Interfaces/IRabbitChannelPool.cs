﻿using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Pools
{
    /// <summary>
    /// CookedRabbit RabbitChannelPool creates the connection pool and manages the channels.
    /// </summary>
    public interface IRabbitChannelPool
    {
        /// <summary>
        /// Check to see if the channel pool has already been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// RabbitChannelPool initialization method.
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <returns></returns>
        Task Initialize(RabbitSeasoning rabbitSeasoning);

        /// <summary>
        /// Manually set the IRabbitConnectionPool for flexiblity in sharing the connection pool across services.
        /// <para>If RabbitConnectionPool is not initialized, it will be here.</para>
        /// <para>If RabbitChannelPool (this) is not initialized, it will be here.</para>
        /// </summary>
        /// <param name="rabbitSeasoning"></param>
        /// <param name="rcp"></param>
        /// <returns></returns>
        Task SetConnectionPoolAsync(RabbitSeasoning rabbitSeasoning, IRabbitConnectionPool rcp);

        /// <summary>
        /// Gets the current channel count by reading the current ChannelId.
        /// </summary>
        /// <returns></returns>
        long GetCurrentChannelCount();

        /// <summary>
        /// Gets the number of times AutoScaling was triggered in the channel pool.
        /// </summary>
        /// <returns></returns>
        long GetChannelPoolAutoScalingIterationCount();

        /// <summary>
        /// Gets the number of times AutoScaling was triggered in the ackable channel pool.
        /// </summary>
        /// <returns></returns>
        long GetAckableChannelPoolAutoScalingIterationCount();

        /// <summary>
        /// Creates a transient (untracked) RabbitMQ channel. Closing/Disposal is the responsibility of the calling service.
        /// </summary>
        /// <param name="enableAck"></param>
        /// <returns>An IModel channel (RabbitMQ).</returns>
        Task<IModel> GetTransientChannelAsync(bool enableAck = false);

        /// <summary>
        /// Gets a pre-created (tracked) RabbitMQ channel. Must be returned by the calling the service!
        /// </summary>
        /// <returns>Returns a ValueTuple(ulong, IModel)</returns>
        Task<(ulong ChannelId, IModel Channel)> GetPooledChannelPairAsync();

        /// <summary>
        /// Gets a pre-created (tracked) RabbitMQ channel that can acknowledge messages. Must be returned by the calling the service!
        /// </summary>
        /// <returns>A ValueTuple(ulong ChannelId, IModel Channel)</returns>
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
        /// <returns>A bool indicating success or failure.</returns>
        bool ReturnChannelToPool((ulong ChannelId, IModel Channel) ChannelPair);

        /// <summary>
        /// Called to return a channel (ackable) to its channel pool.
        /// </summary>
        /// <param name="ChannelPair"></param>
        /// <returns>A bool indicating success or failure.</returns>
        bool ReturnChannelToAckPool((ulong ChannelId, IModel Channel) ChannelPair);

        /// <summary>
        /// Allows for connections to be manually closed.
        /// </summary>
        void CloseConnections();

        /// <summary>
        /// RabbitChannelPool shutdown method closes all channels, disposes each model, and refreshes the objects in memory.
        /// </summary>
        void Shutdown();
    }
}
