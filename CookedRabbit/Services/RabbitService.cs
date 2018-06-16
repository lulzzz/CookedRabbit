using CookedRabbit.Pools;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
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
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPair();

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
                await Console.Out.WriteLineAsync(ace.Message);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
            }

            return success;
        }

        public async Task<List<int>> PublishManyAsync(string queueName, List<byte[]> payloads)
        {
            var failures = new List<int>();
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPair();

            var count = 0;
            foreach (var payload in payloads)
            {
                if(!await PublishAsync(queueName, payload))
                {
                    failures.Add(count);
                }

                count++;
            }

            return failures;
        }

        #endregion

        #region BasicGet Section

        public async Task<BasicGetResult> GetAsync(string queueName)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPair();

            BasicGetResult result = null;

            try
            { result = Channel.BasicGet(queue: queueName, autoAck: true); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            return result;
        }

        public async Task<List<BasicGetResult>> GetManyAsync(string queueName, int batchCount)
        {
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPair();

            uint queueCount = 0;

            try { queueCount = Channel.MessageCount(queueName); }
            catch (Exception e)
            { await Console.Out.WriteLineAsync(e.Message); }

            int resultCount = 0;
            var results = new List<BasicGetResult>();

            if (queueCount != 0)
            {
                while (queueCount > 0 && resultCount <= batchCount)
                {
                    try
                    {
                        results.Add(Channel.BasicGet(queue: queueName, autoAck: true));
                        resultCount++;

                        if (queueCount - resultCount <= 0)
                        { break; }
                    }
                    catch (Exception e)
                    { await Console.Out.WriteLineAsync(e.Message); }
                }
            }

            return results;
        }

        #endregion
    }
}
