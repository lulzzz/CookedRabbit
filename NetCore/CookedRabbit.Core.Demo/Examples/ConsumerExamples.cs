﻿using CookedRabbit.Core.Library.Services;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static CookedRabbit.Core.Demo.DemoHelper;

namespace CookedRabbit.Core.Demo
{
    public class ConsumerExamples
    {
        #region RabbitService w/ Accuracy & Delay Acknowledge Consumer

        private static RabbitService _rabbitService;
        private static ConcurrentDictionary<string, bool> _accuracyCheck = new ConcurrentDictionary<string, bool>();
        private static EventingBasicConsumer consumer = null; // Sits and listens for messages
        private static readonly uint SendLimit = 1000000;
        private static uint ReceiveCount = 0;

        public static async Task RunRabbitServiceConsumerAckTestAsync()
        {
            _rabbitService = new RabbitService("localhost", Environment.MachineName);

            consumer = await _rabbitService.CreateConsumerAsync(ActionWork, queueName);
            await RabbitService_SendMessagesWithLimitWithAccuracyAsync();
            await Console.Out.WriteLineAsync("Finished sending messages.");
        }

        public static async Task RabbitService_SendMessagesWithLimitWithAccuracyAsync()
        {
            var count = 0;
            while (count < SendLimit)
            {
                //await Task.Delay(rand.Next(0, 2)); // Throttle Option

                var message = $"{helloWorld} {count}";
                _accuracyCheck.TryAdd(message, false);

                var task1 = _rabbitService.PublishAsync(queueName, Encoding.UTF8.GetBytes(message));

                await Task.WhenAll(new Task[] { task1 });

                count++;
            }
        }

        public static Action<object, BasicDeliverEventArgs> ActionWork = (o, ea) =>
        {
            ReceiveCount++;
            var message = Encoding.UTF8.GetString(ea.Body);
            if (_accuracyCheck.ContainsKey(message))
            { _accuracyCheck[message] = true; }

            if (o is EventingBasicConsumer consumer)
            {
                try
                { consumer.Model?.BasicAck(ea.DeliveryTag, false); }
                catch (Exception ex)
                { Console.WriteLine($"Error acking messager in consumer. {ex.Message}"); }
            }

            if (ReceiveCount % 100000 == 0)
            { Console.WriteLine($"Received and acked {ReceiveCount} messages."); }
        };

        #endregion

        #region RabbitService w/ Accuracy, Ack, and Redeliver Consumer

        public static async Task RunRabbitServiceConsumerRetryTestAsync()
        {
            _rabbitService = new RabbitService("localhost", Environment.MachineName);

            consumer = await _rabbitService.CreateConsumerAsync(ActionRejectWork, queueName);
            await RabbitService_SendManyWithLimitAsync();
            await Console.Out.WriteLineAsync("Finished sending messages.");
        }

        public static async Task RabbitService_SendManyWithLimitAsync()
        {
            var count = 0;
            var payloads = new List<byte[]>();

            while (count < SendLimit)
            {
                //await Task.Delay(rand.Next(0, 2)); // Throttle Option
                var message = $"{helloWorld} {count}";
                _accuracyCheck.TryAdd(message, false);
                payloads.Add(Encoding.UTF8.GetBytes(message));
                count++;
            }

            var task1 = _rabbitService.PublishManyAsync(queueName, payloads); // Blocks transmission

            await Task.WhenAll(new Task[] { task1 });
        }

        public static Action<object, BasicDeliverEventArgs> ActionRejectWork = (o, ea) =>
        {
            ReceiveCount++;
            var message = Encoding.UTF8.GetString(ea.Body);
            if (_accuracyCheck.ContainsKey(message))
            {
                _accuracyCheck[message] = true;
            }

            if (o is EventingBasicConsumer consumer)
            {
                if (ReceiveCount % 10001 == 0)
                {
                    Console.WriteLine($"Error processing messager in consumer, retrying.");

                    try
                    { consumer.Model?.BasicNack(ea.DeliveryTag, false, true); }
                    catch (Exception ex)
                    { Console.WriteLine($"Error nacking messager in consumer. {ex.Message}"); }
                }
                else
                {
                    try
                    { consumer.Model?.BasicAck(ea.DeliveryTag, false); }
                    catch (Exception ex)
                    { Console.WriteLine($"Error acking messager in consumer. {ex.Message}"); }

                    if (ReceiveCount % 100000 == 0)
                    { Console.WriteLine($"Received and acked {ReceiveCount} messages."); }
                }
            }
        };

        #endregion

        #region RabbitService w/ Publish in Batch & Consumer

        public static async Task RunRabbitServiceBatchPublishWithConsumerTestAsync()
        {
            _rabbitService = new RabbitService("localhost", Environment.MachineName);

            consumer = await _rabbitService.CreateConsumerAsync(ActionRejectWork, queueName);
            await RabbitService_SendManyInBatchesWithLimitAsync();
            await Console.Out.WriteLineAsync("Finished sending messages.");
        }

        public static async Task RabbitService_SendManyInBatchesWithLimitAsync()
        {
            var count = 0;
            var payloads = new List<byte[]>();

            while (count < SendLimit)
            {
                //await Task.Delay(rand.Next(0, 2)); // Throttle Option
                var message = $"{helloWorld} {count}";
                _accuracyCheck.TryAdd(message, false);
                payloads.Add(Encoding.UTF8.GetBytes(message));
                count++;
            }

            var task1 = _rabbitService.PublishManyAsBatchesAsync(queueName, payloads);

            await Task.WhenAll(new Task[] { task1 });
        }

        #endregion
    }
}