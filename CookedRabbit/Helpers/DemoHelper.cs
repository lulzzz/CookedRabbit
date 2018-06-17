using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CookedRabbit
{
    public static class DemoHelper
    {
        #region Example Variables

        public static long TotalMemoryInBytesAtStart = GC.GetTotalMemory(false);

        public static Task cleanupTask;
        public static object _modifyDictionary = new object();
        public static Dictionary<IModel, int> models = new Dictionary<IModel, int>();

        public static Task cleanupTask2;
        public static object _modifyDictionary2 = new object();
        public static Dictionary<IModel, int> models2 = new Dictionary<IModel, int>();

        public static SemaphoreSlim slimShady3 = new SemaphoreSlim(1, 1);
        public static Task cleanupTask3;
        public static Task maintenanceTask3;
        public static Dictionary<IModel, int> models3 = new Dictionary<IModel, int>();

        public static SemaphoreSlim slimShady4 = new SemaphoreSlim(1, 1);
        public static Task cleanupTask4;
        public static Task maintenanceTask4;
        public static IDictionary<IModel, int> models4 = new ConcurrentDictionary<IModel, int>();

        public static Task cleanupTask5;
        public static IDictionary<IModel, int> models5 = new ConcurrentDictionary<IModel, int>();

        public static Task cleanupTask6;
        public static IDictionary<IModel, int> models6 = new ConcurrentDictionary<IModel, int>();

        public static Task cleanupTask7;
        public static IDictionary<IModel, int> models7 = new ConcurrentDictionary<IModel, int>();

        public readonly static string helloWorld = "Hello World! Count:";
        public readonly static string happyShutdown = "Happy shutdown.";

        public readonly static string queueName = "001";

        #endregion

        #region Connection & Channels

        public static Task<ConnectionFactory> CreateChannelFactoryAsync()
        {
            return Task.FromResult(
                new ConnectionFactory
                {
                    HostName = "localhost",

                });
        }

        public static Task<IConnection> CreateConnection(IConnectionFactory connectionFactory, string connectionName = null)
        {
            return Task.FromResult(connectionFactory.CreateConnection(connectionName));
        }

        public static Task<IModel> CreateChannel(IConnection connection)
        {
            return Task.FromResult(connection.CreateModel());
        }

        public static async Task DeclareQueueAsync(IModel channel)
        {
            await Task.Run(() =>
            {
                channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            });
        }

        #endregion

        #region Send Message Helpers

        public static async Task SendMessageAsync(IModel channel, int count = 0)
        {
            await Task.Run(() =>
            {
                string message = $"Hello World! Count: {count}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
            });
        }

        public static async Task SendMessageAndDisposeAsync(IModel channel, int count = 0)
        {
            await Task.Run(() =>
            {
                string message = $"{helloWorld} {count}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
            });

            channel.Close(200, happyShutdown);
            channel.Dispose();
        }

        public static async Task SendMessagesAsync(List<byte[]> payloads, IModel channel)
        {
            await Task.Run(() =>
            {
                foreach (var payload in payloads)
                {
                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: payload);
                }
            });
        }

        public static async Task SendMessageAsync(IConnection connection, int counter)
        {
            ResetThreadName(Thread.CurrentThread, $"SendMessage #{counter}");
            var channel = await CreateChannel(connection);

            models.Add(channel, counter);

            //await Task.Delay(250);
            await SendMessageAsync(channel, counter);

            //await Console.Out.WriteLineAsync($"Iteration {counter} models Count: {models.Count}");

            channel.Dispose();
        }

        public static async Task SendMessageWithBytesAsync(IModel channel, byte[] data)
        {
            await Task.Run(() =>
            {
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: data);
            });
        }

        public static async Task SendMessageLocksAsync(IConnection connection, int counter)
        {
            //ResetThreadName(Thread.CurrentThread, $"SendMessage #{counter}");
            var channel = await CreateChannel(connection);

            lock (_modifyDictionary2)
            { models2.Add(channel, counter); }

            //await Task.Delay(250);
            await SendMessageAsync(channel, counter);

            //await Console.Out.WriteLineAsync($"Iteration {counter} models Count: {models2.Count}");

            channel.Dispose();
        }

        public static async Task SendMessageSemaphoreAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            await slimShady3.WaitAsync();
            models3.Add(channel, counter);
            slimShady3.Release();

            await SendMessageAsync(channel, counter);

            channel.Dispose();
        }

        public static async Task SendMessageSemaphoreWithBytesAsync(IConnection connection, int counter, byte[] bytes)
        {
            var channel = await CreateChannel(connection);

            await slimShady4.WaitAsync();
            models4.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.
            slimShady4.Release();

            await Task.Delay(rand.Next(10, 100)); // Simulate network connectivity
            await SendMessageWithBytesAsync(channel, bytes);

            channel.Dispose();
        }

        public static async Task SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(IConnection connection, int counter, byte[] bytes)
        {
            var channel = await CreateChannel(connection);

            models5.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.

            await Task.Delay(rand.Next(10, 100)); // Simulate network connectivity
            await SendMessageWithBytesAsync(channel, bytes);

            channel.Dispose();
        }

        public static async Task SendMessagesForeverWithConcurrentDictionaryAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            models6.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.

            await SendMessageAsync(channel, counter);

            channel.Dispose();
        }

        #endregion

        #region Receive Messages

        public static async Task<BasicGetResult> ReceiveMessageAsync(IModel channel)
        {
            return await Task.Run(() => { return channel.BasicGet(queue: queueName, autoAck: true); });
        }

        public static async Task<BasicGetResult> ReceiveMessageAndDisposeAsync(IModel channel)
        {
            var result = await Task.Run(() => { return channel.BasicGet(queue: queueName, autoAck: true); });

            channel.Close(200, happyShutdown);
            channel.Dispose();
            return result;
        }

        public static async Task<BasicGetResult> ReceiveMessageAsync(IConnection connection, int counter)
        {
            ResetThreadName(Thread.CurrentThread, $"ReceiveMessage #{counter}");
            var channel = await CreateChannel(connection);

            models.Add(channel, counter);

            //await Task.Delay(100);

            var result = await ReceiveMessageAsync(channel);

            channel.Dispose();

            return result;
        }

        public static async Task ReceiveMessageLocksAsync(IConnection connection, int counter)
        {
            //ResetThreadName(Thread.CurrentThread, $"ReceiveMessage #{counter}");
            var channel = await CreateChannel(connection);

            lock (_modifyDictionary2)
            { models2.Add(channel, counter); }

            //await Task.Delay(100);

            var result = await ReceiveMessageAsync(channel);

            channel.Dispose();
        }

        public static async Task ReceiveMessagesSemaphoreAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            await slimShady3.WaitAsync();
            models3.Add(channel, counter);
            slimShady3.Release();

            await ReceiveMessageAsync(channel);

            channel.Dispose();
        }

        public static async Task ReceiveMessagesSemaphoreWithBytesAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            await slimShady4.WaitAsync();
            models4.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.
            slimShady4.Release();

            await Task.Delay(rand.Next(10, 100)); // Simulate network connectivity
            await ReceiveMessageAsync(channel);

            //await Console.Out.WriteLineAsync($"Iteration {counter} models Count: {models3.Count}");

            channel.Dispose();
        }

        public static async Task ReceiveMessagesConcurrentDictionaryWithBytesAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            models5.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.

            await Task.Delay(rand.Next(10, 100)); // Simulate network connectivity
            await ReceiveMessageAsync(channel);

            channel.Dispose();
        }

        public static async Task ReceiveMessagesConcurrentDictionaryAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            models6.Add(channel, counter); // re-simulate memoryleak by switching just this to models3.

            await ReceiveMessageAsync(channel);

            channel.Dispose();
        }

        #endregion

        #region Miscellaneous

        public static void ResetThreadName(Thread thread, string newName)
        {
            lock (thread)
            {
                var field = thread.GetType().GetField("m_Name", BindingFlags.Instance | BindingFlags.NonPublic);
                if (null != field)
                {
                    field.SetValue(thread, null);
                    thread.Name = null;
                }
            }

            thread.Name = newName;
        }

        public static async Task WarmupAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            var channel = await CreateChannel(connection);

            await DeclareQueueAsync(channel);
            await SendMessageAsync(channel);

            var result = await ReceiveMessageAsync(channel);
            await Console.Out.WriteLineAsync("Program running.");
            channel.Close(200, happyShutdown);
            channel.Dispose();

            connection.Close(200, happyShutdown);
            connection.Dispose();

            connectionFactory = null;
        }

        #region Random Data Generation

        public static async Task<List<byte[]>> CreatePayloadsAsync(int payloadCount)
        {
            var byteList = new List<byte[]>();

            for (int i = 0; i < payloadCount; i++)
            {
                byteList.Add(await GetRandomByteArray());
            }

            return byteList;
        }

        private static Random rand = new Random();
        private static uint x;
        private static uint y;
        private static uint z;
        private static uint w;

        public static async Task<byte[]> GetRandomByteArray(int sizeInBytes = 10000)
        {
            var bytes = new byte[sizeInBytes];

            x = (uint)rand.Next(0, 1000);
            y = (uint)rand.Next(0, 1000);
            z = (uint)rand.Next(0, 1000);
            w = (uint)rand.Next(0, 1000);
            await FillBuffer(bytes, 0, sizeInBytes);

            return bytes;
        }

        public static Task FillBuffer(byte[] buf, int offset, int offsetEnd)
        {
            while (offset < offsetEnd)
            {
                uint t = x ^ (x << 11);
                x = y; y = z; z = w;
                w = w ^ (w >> 19) ^ (t ^ (t >> 8));
                buf[offset++] = (byte)(w & 0xFF);
                buf[offset++] = (byte)((w >> 8) & 0xFF);
                buf[offset++] = (byte)((w >> 16) & 0xFF);
                buf[offset++] = (byte)((w >> 24) & 0xFF);
            }

            return Task.CompletedTask;
        }

        #endregion

        #endregion
    }
}
