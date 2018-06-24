using CookedRabbit.Library.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    public interface IRabbitService
    {
        Task<bool> PublishAsync(string exchangeName, string queueName, byte[] payload);
        Task<List<int>> PublishManyAsync(string exchangeName, string queueName, List<byte[]> payloads);
        Task<List<int>> PublishManyAsBatchesAsync(string exchangeName, string queueName, List<byte[]> payloads);

        Task<BasicGetResult> GetAsync(string queueName);
        Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount);

        Task<(IModel Channel, BasicGetResult Result)> GetWithManualAckAsync(string queueName);
        Task<(IModel ChannelId, List<BasicGetResult> Results)> GetManyWithManualAckAsync(string queueName, int batchCount);

        Task<AckableResult> GetAckableAsync(string queueName);
        Task<AckableResult> GetManyAckableAsync(string queueName, int batchCount);

        Task<EventingBasicConsumer> CreateConsumerAsync(Action<object, BasicDeliverEventArgs> ActionWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);
        Task<AsyncEventingBasicConsumer> CreateAsynchronousConsumerAsync(Func<object, BasicDeliverEventArgs, Task> AsyncWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);
    }
}
