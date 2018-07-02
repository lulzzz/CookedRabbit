using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    /// <summary>
    /// CookedRabbit service for creating Exchanges, Queues, and creating Bindings.
    /// </summary>
    public interface IRabbitTopologyService
    {
        /// <summary>
        /// Create a queue asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="durable"></param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> QueueDeclareAsync(string queueName, bool durable = true, bool exclusive = false,
            bool autoDelete = false, IDictionary<string, object> args = null);

        /// <summary>
        /// Delete a queue asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="onlyIfUnused"></param>
        /// <param name="onlyIfEmpty"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> QueueDeleteAsync(string queueName, bool onlyIfUnused = false, bool onlyIfEmpty = false);

        /// <summary>
        /// Bind a queue to exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> QueueBindToExchangeAsync(string queueName, string exchangeName, string routingKey = "",
            IDictionary<string, object> args = null);

        /// <summary>
        /// Unbind a queue from Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> QueueUnbindFromExchangeAsync(string queueName, string exchangeName, string routingKey = "",
            IDictionary<string, object> args = null);


        /// <summary>
        /// Create an Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> ExchangeDeclareAsync(string exchangeName, string exchangeType, bool durable = true,
            bool autoDelete = false, IDictionary<string, object> args = null);

        /// <summary>
        /// Delete an Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="onlyIfUnused"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> ExchangeDeleteAsync(string exchangeName, bool onlyIfUnused = false);

        /// <summary>
        /// Bind an Exchange to another Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="childExchangeName"></param>
        /// <param name="parentExchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> ExchangeBindToExchangeAsync(string childExchangeName, string parentExchangeName, string routingKey = "",
            IDictionary<string, object> args = null);

        /// <summary>
        /// Unbind an Exchange from another Exchange asynchronously.
        /// <para>Returns success or failure.</para>
        /// </summary>
        /// <param name="childExchangeName"></param>
        /// <param name="parentExchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="args"></param>
        /// <returns>Returns bool indicating success or failure.</returns>
        Task<bool> ExchangeUnbindToExchangeAsync(string childExchangeName, string parentExchangeName, string routingKey = "",
            IDictionary<string, object> args = null);
    }
}
