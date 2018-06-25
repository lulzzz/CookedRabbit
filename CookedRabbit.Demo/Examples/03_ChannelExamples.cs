using CookedRabbit.Library.Models;
using CookedRabbit.Library.Pools;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CookedRabbit.Demo.DemoHelper;

namespace CookedRabbit.Demo
{
    public static class ChannelExamples
    {
        #region Demonstrating a Connection Pool

        private static readonly RabbitSeasoning _rabbitSeasoning = new RabbitSeasoning { RabbitHost = "localhost", LocalHostName = Environment.MachineName };

        public static async Task RunManualTransientChannelTestAsync()
        {
            var rcp = await RabbitConnectionPool.CreateRabbitConnectionPoolAsync(_rabbitSeasoning);

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
            var rcp = await RabbitChannelPool.CreateRabbitChannelPoolAsync(_rabbitSeasoning);

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
                var chanPair1 = await rcp.GetPooledChannelPairAsync();
                var chanPair2 = await rcp.GetPooledChannelPairAsync();
                var chanPair3 = await rcp.GetPooledChannelPairAsync();

                var task1 = SendMessageAsync(chanPair1.Channel, counter++);
                var task2 = SendMessageAsync(chanPair2.Channel, counter++);
                var task3 = SendMessageAsync(chanPair3.Channel, counter++);

                await Task.WhenAll(new Task[] { task1, task2, task3 });

                rcp.ReturnChannelToPool(chanPair1);
                rcp.ReturnChannelToPool(chanPair2);
                rcp.ReturnChannelToPool(chanPair3);

                await Task.Delay(1); // Optional Throttle
            }
        }

        public static async Task PoolChannel_ReceiveMessagesForeverAsync(RabbitChannelPool rcp)
        {
            ResetThreadName(Thread.CurrentThread, "PoolChannel ReceiveMessagesForever Thread");
            while (true)
            {
                var chanPair1 = await rcp.GetPooledChannelPairAsync();
                if (chanPair1.Channel.MessageCount(queueName) > 0)
                {
                    var chanPair2 = await rcp.GetPooledChannelPairAsync();
                    var chanPair3 = await rcp.GetPooledChannelPairAsync();
                    var chanPair4 = await rcp.GetPooledChannelPairAsync();
                    var chanPair5 = await rcp.GetPooledChannelPairAsync();
                    var chanPair6 = await rcp.GetPooledChannelPairAsync();

                    var task1 = ReceiveMessageAsync(chanPair1.Channel);
                    var task2 = ReceiveMessageAsync(chanPair2.Channel);
                    var task3 = ReceiveMessageAsync(chanPair3.Channel);
                    var task4 = ReceiveMessageAsync(chanPair4.Channel);
                    var task5 = ReceiveMessageAsync(chanPair5.Channel);
                    var task6 = ReceiveMessageAsync(chanPair6.Channel);

                    await Task.WhenAll(new Task[] { task1, task2, task3, task4, task5, task6 });

                    rcp.ReturnChannelToPool(chanPair1);
                    rcp.ReturnChannelToPool(chanPair2);
                    rcp.ReturnChannelToPool(chanPair3);
                    rcp.ReturnChannelToPool(chanPair4);
                    rcp.ReturnChannelToPool(chanPair5);
                    rcp.ReturnChannelToPool(chanPair6);

                    await Task.Delay(1); // Optional Throttle
                }
            }
        }

        #endregion
    }
}
