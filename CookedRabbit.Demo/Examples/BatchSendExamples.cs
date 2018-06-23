using RabbitMQ.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using static CookedRabbit.Demo.DemoHelper;

namespace CookedRabbit.Demo
{
    public static class BatchSendExamples
    {
        #region Cross Thread Channels #1 - Concurrent Dictionary (No SemaphoreSlim) Re-use Channel

        public static async Task RunCrossThreadChannelsOneAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory, "SemaphoreRealPayloadConcurrentTestConnection");
            cleanupTask7 = CleanupDictionariesWithConcurrentDictionaryAndSingleChannelAsync(new TimeSpan(0, 0, 20));

            var send = SendMessagesForeverWithConcurrentDictionaryAndSingleChannelAsync(connection);
            var receive = ReceiveMessagesForeverWithConcurrentDictionaryAndSingleChannelAsync(connection);

            await Task.WhenAll(new Task[] { send, receive });
        }

        public static async Task CleanupDictionariesWithConcurrentDictionaryAndSingleChannelAsync(TimeSpan timeSpan)
        {
            while (true)
            {
                await Task.Delay(timeSpan);

                var count = 0;
                var listOfItemsToRemove = models7.Where(x => x.Key.IsClosed).ToArray();
                foreach (var key in listOfItemsToRemove)
                {
                    models7.Remove(key.Key);
                    count++;
                }
                await Console.Out.WriteLineAsync($"Dead channels removed: {count}");
            }
        }

        public static async Task SendMessagesForeverWithConcurrentDictionaryAndSingleChannelAsync(IConnection connection)
        {
            int counter = 0;
            var channel = await CreateChannel(connection);
            models7.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.
            while (true)
            {
                var sendMessages1 = SendMessageAsync(channel, counter++);
                var sendMessages2 = SendMessageAsync(channel, counter++);
                var sendMessages3 = SendMessageAsync(channel, counter++);
                var sendMessages4 = SendMessageAsync(channel, counter++);
                var sendMessages5 = SendMessageAsync(channel, counter++);

                await Task.WhenAll(new Task[] { sendMessages1, sendMessages2, sendMessages3, sendMessages4, sendMessages5 });
            }
            //channel.Dispose();
        }

        public static async Task ReceiveMessagesForeverWithConcurrentDictionaryAndSingleChannelAsync(IConnection connection)
        {
            int counter = 0;
            var channel = await CreateChannel(connection);
            models7.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.
            while (true)
            {
                var receiveMessages1 = ReceiveMessageAsync(channel);
                var receiveMessages2 = ReceiveMessageAsync(channel);
                var receiveMessages3 = ReceiveMessageAsync(channel);
                var receiveMessages4 = ReceiveMessageAsync(channel);
                var receiveMessages5 = ReceiveMessageAsync(channel);
                var receiveMessages6 = ReceiveMessageAsync(channel);
                var receiveMessages7 = ReceiveMessageAsync(channel);
                var receiveMessages8 = ReceiveMessageAsync(channel);

                await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
            }
            //channel.Dispose();
        }

        #endregion

        #region Non-Cross Thread Channels #1 - Re-use Channel

        public static async Task RunNonCrossThreadChannelsAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory, "NonCrossThreadChannels");

            var send = SendMessagesForeverAsync(connection);
            var receive = ReceiveMessagesForeverAsync(connection);

            await Task.WhenAll(new Task[] { send, receive });
        }

        public static async Task SendMessagesForeverAsync(IConnection connection)
        {
            var channel = await CreateChannel(connection);

            while (true)
            {
                var payloads = await CreatePayloadsAsync(100);
                var sendMessages1 = SendMessagesAsync(payloads, channel);

                await Task.WhenAll(new Task[] { sendMessages1 });
                await Task.Delay(100);
            }
        }

        public static async Task ReceiveMessagesForeverAsync(IConnection connection)
        {
            var channel = await CreateChannel(connection);

            while (true)
            {
                var messageCount = channel.MessageCount(queueName);
                await Console.Out.WriteLineAsync($"Message count threshold reached: {messageCount}");
                if (messageCount > 10)
                {
                    var receiveMessages1 = ReceiveMessageAsync(channel);
                    var receiveMessages2 = ReceiveMessageAsync(channel);
                    var receiveMessages3 = ReceiveMessageAsync(channel);
                    var receiveMessages4 = ReceiveMessageAsync(channel);
                    var receiveMessages5 = ReceiveMessageAsync(channel);
                    var receiveMessages6 = ReceiveMessageAsync(channel);
                    var receiveMessages7 = ReceiveMessageAsync(channel);
                    var receiveMessages8 = ReceiveMessageAsync(channel);

                    await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
                }
                await Task.Delay(10);
            }
        }

        #endregion
    }
}
