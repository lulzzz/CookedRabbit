﻿using CookedRabbit.Pools;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            {
                await Console.Out.WriteLineAsync(e.Demystify().Message);
            }

            return success;
        }

        public async Task<List<int>> PublishManyAsync(string queueName, List<byte[]> payloads)
        {
            var failures = new List<int>();
            var (ChannelId, Channel) = await _rcp.GetPooledChannelPairAsync();

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

        #endregion
    }
}