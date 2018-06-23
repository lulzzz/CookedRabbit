using CookedRabbit.Models;
using CookedRabbit.Pools;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CookedRabbit.Services
{
    public class RabbitService : IRabbitService
    {
        private readonly RabbitChannelPool _rcp = null;

        public RabbitService(string rabbitHostName, string localHostName)
        {
            _rcp = RabbitChannelPool.CreateRabbitChannelPoolAsync(rabbitHostName, localHostName).GetAwaiter().GetResult();
        }

        #region BasicPublish Section

        public async Task<bool> PublishAsync(string queueName, byte[] payload)
        {
            var success = false;
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairAsync();

            try
            {
                Channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     false,
                                     basicProperties: null,
                                     body: payload);

                success = true;
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
            {
                _rcp.FlagDeadChannel(ChannelId);
                await Console.Out.WriteLineAsync(ace.Demystify().Message);
            }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Demystify().Message); }

            return success;
        }

        public async Task<List<int>> PublishManyAsync(string queueName, List<byte[]> payloads)
        {
            var failures = new List<int>();
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairAsync();
            var rand = new Random();
            foreach (var payload in payloads)
            {
                try
                {
                    Channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         false,
                                         basicProperties: null,
                                         body: payload);

                    await Task.Delay(rand.Next(0,1));
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                {
                    _rcp.FlagDeadChannel(ChannelId);
                    await Console.Out.WriteLineAsync(ace.Demystify().Message);
                }
                catch (Exception e)
                { await Console.Out.WriteLineAsync(e.Demystify().Message); }
            }

            return failures;
        }

        public async Task<List<int>> PublishManyAsBatchesAsync(string queueName, List<byte[]> payloads)
        {
            var failures = new List<int>();
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairAsync();
            var rand = new Random();

            while (payloads.Any())
            {
                var processingPayloads = payloads.Take(100);
                payloads.RemoveRange(0, payloads.Count > 100 ? 100 : payloads.Count);

                foreach (var payload in processingPayloads)
                {
                    try
                    {
                        Channel.BasicPublish(exchange: "",
                                             routingKey: queueName,
                                             false,
                                             basicProperties: null,
                                             body: payload);
                    }
                    catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                    {
                        _rcp.FlagDeadChannel(ChannelId);
                        await Console.Out.WriteLineAsync(ace.Demystify().Message);
                    }
                    catch (Exception e)
                    { await Console.Out.WriteLineAsync(e.Demystify().Message); }
                }

                await Task.Delay(rand.Next(0, 2));
            }

            return failures;
        }

        #endregion

        #region BasicGet Section

        public async Task<BasicGetResult> GetAsync(string queueName)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairAsync();

            BasicGetResult result = null;

            try
            { result = Channel.BasicGet(queue: queueName, autoAck: true); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Demystify().Message); }

            return result;
        }

        public async Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairAsync();

            uint queueCount = 0;

            try { queueCount = Channel.MessageCount(queueName); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Demystify().Message); }

            int resultCount = 0;
            var results = new List<BasicGetResult>();

            if (queueCount != 0)
            {
                while (queueCount > 0 && resultCount < batchCount)
                {
                    try
                    {
                        var result = Channel.BasicGet(queue: queueName, autoAck: true);
                        if (result == null) //Empty Queue
                        { break; }

                        results.Add(result);
                        resultCount++;
                    }
                    catch (Exception e)
                    { await Console.Out.WriteLineAsync(e.Demystify().Message); }
                }
            }

            return results;
        }

        #endregion

        #region BasicGet With Manual Ack Section

        public async Task<(IModel Channel, BasicGetResult Result)> GetWithManualAckAsync(string queueName)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairWithManualAckAsync();

            BasicGetResult result = null;

            try
            { result = Channel.BasicGet(queue: queueName, autoAck: false); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Demystify().Message); }

            return (Channel, result);
        }

        public async Task<(IModel ChannelId, List<BasicGetResult> Results)> GetManyWithManualAckAsync(string queueName, int batchCount)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairWithManualAckAsync();

            uint queueCount = 0;

            try { queueCount = Channel.MessageCount(queueName); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Demystify().Message); }

            int resultCount = 0;
            var results = new List<BasicGetResult>();

            if (queueCount != 0)
            {
                while (queueCount > 0 && resultCount < batchCount)
                {
                    try
                    {
                        var result = Channel.BasicGet(queue: queueName, autoAck: false);
                        if (result == null) //Empty Queue
                        { break; }

                        results.Add(result);
                        resultCount++;
                    }
                    catch (Exception e)
                    { await Console.Out.WriteLineAsync(e.Demystify().Message); }
                }
            }

            return (Channel, results);
        }

        public async Task<AckableResult> GetAckableAsync(string queueName)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairWithManualAckAsync();

            BasicGetResult result = null;

            try
            { result = Channel.BasicGet(queue: queueName, autoAck: false); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Demystify().Message); }

            return new AckableResult { Channel = Channel, Results = new List<BasicGetResult>() { result } };
        }

        public async Task<AckableResult> GetManyAckableAsync(string queueName, int batchCount)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairWithManualAckAsync();

            uint queueCount = 0;

            try { queueCount = Channel.MessageCount(queueName); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Demystify().Message); }

            int resultCount = 0;
            var results = new List<BasicGetResult>();

            if (queueCount != 0)
            {
                while (queueCount > 0 && resultCount < batchCount)
                {
                    try
                    {
                        var result = Channel.BasicGet(queue: queueName, autoAck: false);
                        if (result == null) //Empty Queue
                        { break; }

                        results.Add(result);
                        resultCount++;
                    }
                    catch (Exception e)
                    { await Console.Out.WriteLineAsync(e.Demystify().Message); }
                }
            }

            return new AckableResult { Channel = Channel, Results = results };
        }

        #endregion

        #region Consumer Section

        public async Task<EventingBasicConsumer> CreateConsumerAsync(
            Action<object, BasicDeliverEventArgs> ActionWork,
            string queueName,
            ushort prefetchCount = 120,
            bool autoAck = false)
        {
            var channel = await _rcp.GetTransientChannelWithManualAckAsync();
            if (channel is null) throw new Exception("Channel was unable to be created for this consumer.");

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(0, 100, false);
            consumer.Received += (model, ea) => ActionWork(model, ea);
            channel.BasicConsume(queue: queueName,
                                 autoAck: autoAck,
                                 consumer: consumer);

            return consumer;
        }

        #endregion
    }
}
