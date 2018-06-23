using CookedRabbit.Core.Library.Pools;
using CookedRabbit.Core.Library.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CookedRabbit.Core.Demo.DemoHelper;

namespace CookedRabbit.Core.Demo
{
    public static class ChannelExamples
    {
        #region Demonstrating a Connection Pool

        public static async Task RunManualTransientChannelTestAsync()
        {
            var rcp = await RabbitConnectionPool.CreateRabbitConnectionPoolAsync("localhost", Environment.CurrentDirectory);

            var sendMessage = SendMessagesForeverAsync(rcp);
            var receiveMessage = ReceiveMessagesForeverAsync(rcp);

            await Task.WhenAll(new Task[] { sendMessage, receiveMessage });
        }

        public static async Task SendMessagesForeverAsync(RabbitConnectionPool rcp)
        {
            ResetThreadName(Thread.CurrentThread, "Transient SendMessagesForever Thread");
            int counter = 0;
            while (true)
            {
                var chan1 = rcp.GetConnection().CreateModel();
                var chan2 = rcp.GetConnection().CreateModel();
                var chan3 = rcp.GetConnection().CreateModel();

                var task1 = SendMessageAsync(chan1, counter++);
                var task2 = SendMessageAsync(chan2, counter++);
                var task3 = SendMessageAsync(chan3, counter++);

                await Task.WhenAll(new Task[] { task1, task2, task3 });

                chan1.Close(200, happyShutdown);
                chan1.Dispose();

                chan2.Close(200, happyShutdown);
                chan2.Dispose();

                chan3.Close(200, happyShutdown);
                chan3.Dispose();
            }
        }

        public static async Task ReceiveMessagesForeverAsync(RabbitConnectionPool rcp)
        {
            ResetThreadName(Thread.CurrentThread, "Transient ReceiveMessagesForever Thread");
            while (true)
            {
                var chan1 = rcp.GetConnection().CreateModel();
                var chan2 = rcp.GetConnection().CreateModel();
                var chan3 = rcp.GetConnection().CreateModel();

                if (chan1.MessageCount(queueName) > 0)
                {
                    var task1 = ReceiveMessageAsync(chan1);
                    var task2 = ReceiveMessageAsync(chan2);
                    var task3 = ReceiveMessageAsync(chan3);

                    await Task.WhenAll(new Task[] { task1, task2, task3 });
                }

                chan1.Close(200, happyShutdown);
                chan1.Dispose();

                chan2.Close(200, happyShutdown);
                chan2.Dispose();

                chan3.Close(200, happyShutdown);
                chan3.Dispose();
            }
        }

        #endregion

        #region Demonstrating a ChannelPool backed by ConnectionPool

        public static async Task RunPoolChannelTestAsync()
        {
            var rcp = await RabbitChannelPool.CreateRabbitChannelPoolAsync("localhost", "localhost");

            var sendMessage = PoolChannel_SendMessagesForeverAsync(rcp);
            var receiveMessage = PoolChannel_ReceiveMessagesForeverAsync(rcp);

            await Task.WhenAll(new Task[] { sendMessage, receiveMessage });
        }

        public static async Task PoolChannel_SendMessagesForeverAsync(RabbitChannelPool rcp)
        {
            ResetThreadName(Thread.CurrentThread, "PoolChannel SendMessagesForever Thread");
            int counter = 0;
            while (true)
            {
                var task1 = SendMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel, counter++);
                var task2 = SendMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel, counter++);
                var task3 = SendMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel, counter++);

                await Task.WhenAll(new Task[] { task1, task2, task3 });
            }
        }

        public static async Task PoolChannel_ReceiveMessagesForeverAsync(RabbitChannelPool rcp)
        {
            ResetThreadName(Thread.CurrentThread, "PoolChannel ReceiveMessagesForever Thread");
            while (true)
            {
                var (ChannelId, Channel) = await rcp.GetPooledChannelPairAsync();
                if (Channel.MessageCount(queueName) > 0)
                {
                    var task1 = ReceiveMessageAsync(Channel);
                    var task2 = ReceiveMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel);
                    var task3 = ReceiveMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel);
                    var task4 = ReceiveMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel);
                    var task5 = ReceiveMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel);
                    var task6 = ReceiveMessageAsync((await rcp.GetPooledChannelPairAsync()).Channel);

                    await Task.WhenAll(new Task[] { task1, task2, task3, task4, task5, task6 });
                }
            }
        }

        #endregion
    }
}
