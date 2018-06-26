using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CookedRabbit.Demo.DemoHelper;

namespace CookedRabbit.Demo
{
    public static class ChannelExamples
    {
        #region Creating Channels

        public static async Task RunCreateChannelAndUseAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            var channel = await CreateChannel(connection);

            await DeclareQueueAsync(channel);
            await SendMessageAsync(channel);

            var result = await ReceiveMessageAsync(channel);

            channel.Close(200, happyShutdown);
            channel.Dispose();

            connection.Close(200, happyShutdown);
            connection.Dispose();

            connectionFactory = null;
        }

        public static async Task RunCreateMultipleChannelsAndUseAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            var channelQueue = new ConcurrentQueue<IModel>();

            await Console.Out.WriteLineAsync("Program running.");

            for (int i = 0; i < 10; i++)
            {
                var channel = await CreateChannel(connection);
                channelQueue.Enqueue(channel);
            }

            while (channelQueue.IsEmpty)
            {
                if (channelQueue.TryDequeue(out IModel channel))
                {
                    await DeclareQueueAsync(channel);
                    await SendMessageAsync(channel);

                    var result = await ReceiveMessageAsync(channel);
                    channel.Close(200, happyShutdown);
                    channel.Dispose();
                }
            }

            connection.Close(200, happyShutdown);
            connection.Dispose();

            connectionFactory = null;
        }

        #endregion

        #region Channel Mistakes

        public static async Task RunCreateChannelAndDoubleDisposeAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            var channel = await CreateChannel(connection);

            await DeclareQueueAsync(channel);
            await SendMessageAsync(channel);

            var result = await ReceiveMessageAsync(channel);

            channel.Close(200, happyShutdown);
            channel.Dispose();
            channel.Dispose();

            connection.Close(200, happyShutdown);
            connection.Dispose();

            connectionFactory = null;
        }

        public static IModel channel;
        public static List<IModel> channels = new List<IModel>();

        public static async Task RunCreateChannelAndUseAfterDisposeAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            channel = await CreateChannel(connection);
            channels.Add(channel);

            await DeclareQueueAsync(channel);
            await SendMessageAsync(channel);

            var result = await ReceiveMessageAsync(channel);

            channel.Close(200, happyShutdown);
            channel.Dispose();

            await SendMessageAsync(channels.ElementAt(0));
            var result2 = await ReceiveMessageAsync(channels.ElementAt(0));

            connection.Close(200, happyShutdown);
            connection.Dispose();

            connectionFactory = null;
        }

        public static async Task RunCreateChannelAndUseAfterUsingStatementAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);

            await Task.Run( async () =>
            {
                using (var channel = await CreateChannel(connection))
                {
                    channels.Add(channel);

                    await DeclareQueueAsync(channel);
                    await SendMessageAsync(channel);

                    var result = await ReceiveMessageAsync(channel);
                }
            });

            GC.Collect();
            await Task.Delay(1000);

            await SendMessageAsync(channels.ElementAt(0));
            var result2 = await ReceiveMessageAsync(channels.ElementAt(0));

            connection.Close(200, happyShutdown);
            connection.Dispose();

            connectionFactory = null;
        }

        #endregion
    }
}
