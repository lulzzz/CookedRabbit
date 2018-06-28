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
        Task<bool> PublishAsync(string exchangeName, string queueName, byte[] payload, IBasicProperties messageProperties = null);
        Task<List<int>> PublishManyAsync(string exchangeName, string queueName, List<byte[]> payloads, IBasicProperties messageProperties = null);
        Task<List<int>> PublishManyAsBatchesAsync(string exchangeName, string queueName, List<byte[]> payloads, ushort batchSize = 100, IBasicProperties messageProperties = null);
        Task<List<int>> PublishManyAsBatchesInParallelAsync(string exchangeName, string queueName, List<byte[]> payloads, ushort batchSize = 100, IBasicProperties messageProperties = null);

        Task<bool> CompressAndPublishAsync(string exchangeName, string queueName, byte[] payload, string contentType, IBasicProperties messageProperties = null);

        Task<BasicGetResult> GetAsync(string queueName);
        Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount);

        Task<byte[]> GetAndDecompressAsync(string queueName);

        Task<(IModel Channel, BasicGetResult Result)> GetWithManualAckAsync(string queueName);
        Task<(IModel Channel, List<BasicGetResult> Results)> GetManyWithManualAckAsync(string queueName, int batchCount);

        Task<AckableResult> GetAckableAsync(string queueName);
        Task<AckableResult> GetManyAckableAsync(string queueName, int batchCount);

        Task<EventingBasicConsumer> CreateConsumerAsync(Action<object, BasicDeliverEventArgs> ActionWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);
        Task<AsyncEventingBasicConsumer> CreateAsynchronousConsumerAsync(Func<object, BasicDeliverEventArgs, Task> AsyncWork, string queueName, ushort prefetchCount = 120, bool autoAck = false);
    }
}
