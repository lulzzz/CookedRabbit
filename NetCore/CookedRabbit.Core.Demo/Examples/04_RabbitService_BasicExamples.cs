using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CookedRabbit.Core.Demo.DemoHelper;
using static CookedRabbit.Core.Library.Utilities.RandomData;

namespace CookedRabbit.Core.Demo
{
    public class RabbitServiceExamples
    {
        #region RabbitService is a RabbitMQ Wrapper

        private static readonly RabbitSeasoning _rabbitSeasoning = new RabbitSeasoning { RabbitHostName = "localhost", ConnectionName = Environment.MachineName };

        // Using RabbitService backed by a Channel Pool
        public static RabbitDeliveryService _rabbitDeliveryService;
        public static Random rand = new Random();

        public static async Task RunRabbitServicePoolChannelTestAsync()
        {
            _rabbitDeliveryService = new RabbitDeliveryService(_rabbitSeasoning);

            await Task.WhenAll(new Task[] { RabbitService_SendMessagesForeverAsync(), RabbitService_ReceiveMessagesForeverAsync() });
        }

        public static async Task RabbitService_SendMessagesForeverAsync()
        {
            ResetThreadName(Thread.CurrentThread, "RabbitService_Send");

            var count = 0;
            while (true)
            {
                await Task.Delay(rand.Next(0, 2));

                var task1 = _rabbitDeliveryService.PublishAsync(exchangeName, queueName, await GetRandomByteArray(4000));
                var task2 = _rabbitDeliveryService.PublishAsync(exchangeName, queueName, await GetRandomByteArray(4000));

                await Task.WhenAll(new Task[] { task1, task2 });

                count++;
            }
        }

        public static async Task RabbitService_ReceiveMessagesForeverAsync()
        {
            ResetThreadName(Thread.CurrentThread, "RabbitService_Receive");
            while (true)
            {
                await Task.Delay(rand.Next(0, 2));

                var task1 = _rabbitDeliveryService.GetManyAsync(queueName, 100);

                await Task.WhenAll(new Task[] { task1 });
            }
        }

        #endregion

        #region RabbitService Accuracy Test

        private static ConcurrentDictionary<string, bool> _accuracyCheck = new ConcurrentDictionary<string, bool>();

        public static async Task RunRabbitServiceAccuracyTestAsync()
        {
            _rabbitDeliveryService = new RabbitDeliveryService(_rabbitSeasoning);

            await Task.WhenAll(new Task[] { RabbitService_ReceiveMessagesForeverWithAccuracyAsync(), RabbitService_SendMessagesForeverWithAccuracyAsync() });
        }

        public static async Task RabbitService_SendMessagesForeverWithAccuracyAsync()
        {
            var count = 0;
            while (true)
            {
                //await Task.Delay(rand.Next(0, 2)); // Throttle Option

                var message = $"{helloWorld} {count}";
                _accuracyCheck.TryAdd(message, false);

                var task1 = _rabbitDeliveryService.PublishAsync(exchangeName, queueName, Encoding.UTF8.GetBytes(message));

                await Task.WhenAll(new Task[] { task1 });

                count++;
            }
        }

        public static async Task RabbitService_ReceiveMessagesForeverWithAccuracyAsync()
        {
            while (true)
            {
                //await Task.Delay(rand.Next(0, 2));  // Throttle Option

                var task1 = _rabbitDeliveryService.GetManyAsync(queueName, 100);
                var task2 = _rabbitDeliveryService.GetManyAsync(queueName, 100);
                var task3 = _rabbitDeliveryService.GetManyAsync(queueName, 100);

                await Task.WhenAll(new Task[] { task1, task2, task3 });

                var results = task1.Result;
                results.AddRange(task2.Result);
                results.AddRange(task3.Result);

                foreach (var result in results)
                {
                    var message = Encoding.UTF8.GetString(result.Body);
                    if (_accuracyCheck.ContainsKey(message))
                    {
                        _accuracyCheck[message] = true;
                    }
                }
            }
        }

        #endregion

        #region RabbitService w/ Delay Acknowledge & Accuracy Test

        public static async Task RunRabbitServiceDelayAckTestAsync()
        {
            _rabbitDeliveryService = new RabbitDeliveryService(_rabbitSeasoning);

            await Task.WhenAll(new Task[] { RabbitService_SendMessagesWithLimitWithAccuracyAsync(), RabbitService_ReceiveMessagesForeverDelayAckAsync() });
        }

        public static async Task RabbitService_SendMessagesWithLimitWithAccuracyAsync()
        {
            var count = 0;
            while (count < 10000)
            {
                //await Task.Delay(rand.Next(0, 2)); // Throttle Option

                var message = $"{helloWorld} {count}";
                _accuracyCheck.TryAdd(message, false);

                var task1 = _rabbitDeliveryService.PublishAsync(exchangeName, queueName, Encoding.UTF8.GetBytes(message));

                await Task.WhenAll(new Task[] { task1 });

                count++;
            }
        }

        public static async Task RabbitService_ReceiveMessagesForeverDelayAckAsync()
        {
            while (true)
            {
                await Task.Delay(rand.Next(0, 2));  // Throttle Option

                (IModel Channel, List<BasicGetResult> Results) batchResult = (null, new List<BasicGetResult>());

                try
                {
                    batchResult = await _rabbitDeliveryService.GetManyWithManualAckAsync(queueName, 100);
                }
                catch (Exception e) { await Console.Out.WriteLineAsync($"Error occurred: {e.Message}"); }


                foreach (var result in batchResult.Results)
                {
                    var success = await DoWork(result.Body);

                    if (success)
                    { batchResult.Channel.BasicAck(result.DeliveryTag, false); }
                    else
                    {
                        await Console.Out.WriteLineAsync($"Message {result.DeliveryTag} failed to deliver, Nacking w/ requeue.");
                        batchResult.Channel.BasicNack(result.DeliveryTag, false, true);
                    }
                }
            }
        }

        private static Task<bool> DoWork(byte[] body)
        {
            var success = false;
            var message = Encoding.UTF8.GetString(body);
            if (_accuracyCheck.ContainsKey(message))
            {
                _accuracyCheck[message] = true;
                success = true;
            }

            return Task.FromResult(success);
        }

        #endregion
    }
}
