using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    public interface IRabbitTopologyService
    {
        Task<bool> QueueDeclareAsync(string queueName, bool durable = true, bool exclusive = false,
                                     bool autoDelete = false, IDictionary<string, object> args = null);

        Task<bool> QueueDeleteAsync(string queueName, bool onlyIfUnused = false, bool onlyIfEmpty = false);

        Task<bool> QueueBindToExchangeAsync(string queueName, string exchangeName, string routingKey = "",
                                            IDictionary<string, object> args = null);

        Task<bool> QueueUnbindFromExchangeAsync(string queueName, string exchangeName, string routingKey = "",
                                                IDictionary<string, object> args = null);


        Task<bool> ExchangeDeclareAsync(string exchangeName, string exchangeType, bool durable = true,
                                        bool autoDelete = false, IDictionary<string, object> args = null);

        Task<bool> ExchangeDeleteAsync(string exchangeName, bool onlyIfUnused = false);

        Task<bool> ExchangeBindToExchangeAsync(string childExchangeName, string parentExchangeName, string routingKey = "",
                                               IDictionary<string, object> args = null);

        Task<bool> ExchangeUnbindToExchangeAsync(string childExchangeName, string parentExchangeName, string routingKey = "",
                                                 IDictionary<string, object> args = null);
    }
}
