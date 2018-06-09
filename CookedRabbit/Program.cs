using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CookedRabbit
{
    public class Program
    {
        public static long TotalMemoryInBytesAtStart = GC.GetTotalMemory(false);

        public static async Task Main(string[] args)
        {
            //await RunBasicSendReceiveExampleAsync();

            //await RunMemoryLeakAsync();

            //await RunMemoryLeakMadeWorseAsync();

            //await RunMemoryLeakFixAttempOneAsync();

            //await RunMemoryLeakFixAttempTwoAsync();

            // To Run, have Erlang 20.3 and Server rabbit 3.7.5 installed locally
            // and running first.
            // Focus on this method to see high performance in concurrent threads using
            // a dictionary that is also being cleaned up without deadlocks or
            // exceptions.
            await RunMemoryLeakFixAttempThreeAsync();

            // Focus on this method to see high IO usage in concurrent threads using
            // a dictionary that is also being cleaned up without deadlocks or
            // exceptions. Real data payloads and send/receives simulate network
            // communication times.
            //await RunMemoryLeakFixAttempTwoAsync();
            await Console.In.ReadLineAsync();
        }

        public static async Task RunBasicSendReceiveExampleAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            var channel = await CreateChannel(connection);


            await DeclareQueueAsync(channel);
            await SendMessageAsync(channel);

            var result = await ReceiveMessageAsync(channel);
            await Console.Out.WriteLineAsync(Encoding.UTF8.GetString(result.Body));
        }

        #region Helpers

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
                channel.QueueDeclare(queue: "001",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            });
        }

        public static async Task SendMessageAsync(IModel channel, int count = 0)
        {
            await Task.Run(() =>
            {
                string message = $"Hello World! Count: {count}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "001",
                                     basicProperties: null,
                                     body: body);
            });
        }

        public static async Task SendMessageWithBytesAsync(IModel channel, int count, byte[] data)
        {
            await Task.Run(() =>
            {
                channel.BasicPublish(exchange: "",
                                     routingKey: "001",
                                     basicProperties: null,
                                     body: data);
            });
        }

        public static async Task<BasicGetResult> ReceiveMessageAsync(IModel channel)
        {
            return await Task.Run(() => { return channel.BasicGet(queue: "001", autoAck: true); });
        }

        #endregion

        #region Reproducing Memoryleak

        static Dictionary<IModel, ConcurrentDictionary<ulong, TaskCompletionSource<ulong>>> ConfirmsDictionary =
                                new Dictionary<IModel, ConcurrentDictionary<ulong, TaskCompletionSource<ulong>>>();
        static ConcurrentDictionary<IModel, object> ChannelLocks = new ConcurrentDictionary<IModel, object>();
        static Dictionary<IModel, ulong> ChannelSequences = new Dictionary<IModel, ulong>();

        public static async Task RunMemoryLeakAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);

            Parallel.For(0, 100, async (i) =>
            {
                var channel = await CreateChannel(connection);
                ChannelSequences.Add(channel, channel.NextPublishSeqNo);

                await SendMessageAsync(channel, i);

                var result = await ReceiveMessageAsync(channel);
                await Console.Out.WriteLineAsync(Encoding.UTF8.GetString(result.Body));
                await Console.Out.WriteLineAsync($"Iteration {i} ChannelSequences Count: {ChannelSequences.Count}");

                channel.Dispose();
                i++;
            });

            foreach (var kvp in ChannelSequences)
            {
                await Console.Out.WriteLineAsync($"WTF AM I: {kvp.Key.GetType()} AND WHAT DO I HOLD: {kvp.Key}");
            }

            for (int i = 0; i < ChannelSequences.Count; i++)
            {
                try
                {
                    var channel = ChannelSequences.ElementAt(i).Key;
                    await SendMessageAsync(channel, i + 1000);
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                {
                    await Console.Out.WriteLineAsync($"AlreadyClosedException #{i.ToString("000")} Reason: {ace.ShutdownReason}");
                }
            }

            for (int i = 0; i < 5; i++)
            {
                await Console.Out.WriteLineAsync($"Iteration {i} ChannelSequences Count {ChannelSequences.Count}");
                await Console.Out.WriteLineAsync($"Memory in KBytes of Application {(GC.GetTotalMemory(false) - TotalMemoryInBytesAtStart) / 1000.0}");
                Thread.Sleep(10000);
                GC.AddMemoryPressure(Environment.SystemPageSize);
                GC.Collect();
            }

            foreach (var kvp in ChannelSequences)
            {
                await Console.Out.WriteLineAsync($"WTF AM I: {kvp.Key.GetType()} AND WHAT DO I HOLD: {kvp.Key}");
            }
        }


        public static async Task RunMemoryLeakMadeWorseAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);

            Parallel.For(0, 100, async (i) =>
            {
                var channel = await CreateChannel(connection);
                ChannelSequences.Add(channel, channel.NextPublishSeqNo);

                await SendMessageAsync(channel, i);

                var result = await ReceiveMessageAsync(channel);
                await Console.Out.WriteLineAsync(Encoding.UTF8.GetString(result.Body));
                await Console.Out.WriteLineAsync($"Iteration {i} ChannelSequences Count: {ChannelSequences.Count}");

                channel.Dispose();
                i++;
            });

            if (ChannelSequences.Count >= 100)
            {
                for (int i = 0; i < 100; i++)
                {
                    var channel = ChannelSequences.ElementAt(i).Key;
                    try
                    {
                        await SendMessageAsync(channel, i + 1000);
                    }
                    catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                    {
                        await Console.Out.WriteLineAsync($"AlreadyClosedException #{i.ToString("000")} Reason: {ace.ShutdownReason}");
                        await Console.Out.WriteLineAsync($"New Channel Created! #{i.ToString("000")} Reason: Old Channel Closed.");
                        channel = await CreateChannel(connection);
                        ChannelSequences.Add(channel, channel.NextPublishSeqNo);

                        await SendMessageAsync(channel, i + 1000);

                        var result = await ReceiveMessageAsync(channel);
                        await Console.Out.WriteLineAsync(Encoding.UTF8.GetString(result.Body));
                        await Console.Out.WriteLineAsync($"Iteration {i} ChannelSequences Count: {ChannelSequences.Count}");

                        channel.Dispose();
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                await Console.Out.WriteLineAsync($"Iteration {i} ChannelSequences Count {ChannelSequences.Count}");
                await Console.Out.WriteLineAsync($"Memory in KBytes of Application {(GC.GetTotalMemory(false) - TotalMemoryInBytesAtStart) / 1000.0}");
                Thread.Sleep(10000);
                GC.AddMemoryPressure(Environment.SystemPageSize);
                GC.Collect();
            }

            foreach (var kvp in ChannelSequences)
            {
                await Console.Out.WriteLineAsync($"WTF AM I: {kvp.Key.GetType()} AND WHAT DO I HOLD: {kvp.Key}");
            }
        }

        #endregion

        #region Memory Leak Work #1

        public static async Task RunMemoryLeakFixAttempOneAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            cleanupTask = CleanupDictionariesAsync(new TimeSpan(0, 0, 20));

            var sendMessage = SendMessageForeverAsync(connection);
            var receiveMessage = ReceiveMessageForeverAsync(connection);

            await Task.WhenAll(new Task[] { sendMessage, receiveMessage });
        }

        public static Task cleanupTask;
        public static object _modifyDictionary = new object();
        public static Dictionary<IModel, int> models = new Dictionary<IModel, int>();

        static private void ResetThreadName(Thread thread, string newName)
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

        public static async Task CleanupDictionariesAsync(TimeSpan timeSpan)
        {
            ResetThreadName(Thread.CurrentThread, "CleanupDictionaries Thread");
            while (true)
            {
                await Task.Delay(timeSpan);

                var listOfItemsToRemove = models.Where(x => x.Key.IsClosed).ToArray();

                foreach (var kvp in listOfItemsToRemove)
                {
                    models.Remove(kvp.Key);
                }

                await Console.Out.WriteLineAsync($"Dead channels removed. Total Connections: {models2.Count}");
            }
        }

        public static async Task SendMessageForeverAsync(IConnection connection)
        {
            ResetThreadName(Thread.CurrentThread, "SendMessagesForever Thread");
            int counter = 0;
            while (true)
            {
                var sendMessageTask1 = SendMessageAsync(connection, counter++);
                var sendMessageTask2 = SendMessageAsync(connection, counter++);
                var sendMessageTask3 = SendMessageAsync(connection, counter++);

                await Task.WhenAll(new Task[] { sendMessageTask1, sendMessageTask2, sendMessageTask3 });
            }
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

        public static async Task<BasicGetResult> ReceiveMessageForeverAsync(IConnection connection)
        {
            ResetThreadName(Thread.CurrentThread, "ReceiveMessageForever Thread");
            int counter = 0;
            while (true)
            {
                var result1 = ReceiveMessageAsync(connection, counter++);
                var result2 = ReceiveMessageAsync(connection, counter++);
                var result3 = ReceiveMessageAsync(connection, counter++);

                await Task.WhenAll(new Task[] { result1, result2, result3 });
                await Console.Out.WriteLineAsync($"Iteration {counter} models Count: {models.Count}");
            }
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

        #endregion

        #region Memory Leak Work #2

        public static async Task RunMemoryLeakFixAttempTwoAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            cleanupTask = CleanupDictionariesWithLocksAsync(new TimeSpan(0, 0, 20));

            var sendMessage = SendMessageForeverWithLocksAsync(connection);
            var receiveMessage = ReceiveMessageForeverWithLocksAsync(connection);

            await Task.WhenAll(new Task[] { sendMessage, receiveMessage });
        }

        public static Task cleanupTask2;
        public static object _modifyDictionary2 = new object();
        public static Dictionary<IModel, int> models2 = new Dictionary<IModel, int>();

        public static async Task CleanupDictionariesWithLocksAsync(TimeSpan timeSpan)
        {
            //ResetThreadName(Thread.CurrentThread, "CleanupDictionaries Thread");
            while (true)
            {
                await Task.Delay(timeSpan);

                lock (_modifyDictionary2)
                {
                    var listOfItemsToRemove = models2.Where(x => x.Key.IsClosed).ToArray();

                    foreach (var kvp in listOfItemsToRemove)
                    {
                        models2.Remove(kvp.Key);
                    }
                }

                await Console.Out.WriteLineAsync($"Dead channels removed. Total Connections: {models2.Count}");
            }
        }

        public static async Task SendMessageForeverWithLocksAsync(IConnection connection)
        {
            //ResetThreadName(Thread.CurrentThread, "SendMessagesForever Thread");
            int counter = 0;
            while (true)
            {
                var sendMessages1 = SendMessageLocksAsync(connection, counter++);
                var sendMessages2 = SendMessageLocksAsync(connection, counter++);
                var sendMessages3 = SendMessageLocksAsync(connection, counter++);
                var sendMessages4 = SendMessageLocksAsync(connection, counter++);
                var sendMessages5 = SendMessageLocksAsync(connection, counter++);

                await Task.WhenAll(new Task[] { sendMessages1, sendMessages2, sendMessages3, sendMessages4, sendMessages5 });
            }
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

        public static async Task<BasicGetResult> ReceiveMessageForeverWithLocksAsync(IConnection connection)
        {
            //ResetThreadName(Thread.CurrentThread, "ReceiveMessageForever Thread");
            int counter = 0;
            while (true)
            {
                var receiveMessages1 = ReceiveMessageLocksAsync(connection, counter++);
                var receiveMessages2 = ReceiveMessageLocksAsync(connection, counter++);
                var receiveMessages3 = ReceiveMessageLocksAsync(connection, counter++);
                var receiveMessages4 = ReceiveMessageLocksAsync(connection, counter++);
                var receiveMessages5 = ReceiveMessageLocksAsync(connection, counter++);
                var receiveMessages6 = ReceiveMessageLocksAsync(connection, counter++);
                var receiveMessages7 = ReceiveMessageLocksAsync(connection, counter++);
                var receiveMessages8 = ReceiveMessageLocksAsync(connection, counter++);

                await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
            }
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

        #endregion

        #region Memory Leak Work #3

        public static SemaphoreSlim slimShady3 = new SemaphoreSlim(1, 1);
        public static Task cleanupTask3;
        public static Task maintenanceTask3;
        public static Dictionary<IModel, int> models3 = new Dictionary<IModel, int>();

        public static async Task RunMemoryLeakFixAttempThreeAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory, "SemaphorePerformanceTestConnection");
            cleanupTask3 = CleanupDictionariesWithSemaphoreAsync(new TimeSpan(0, 0, 20));
            //maintenanceTask3 = MaintenanceWithSemaphoreAsync(new TimeSpan(0, 1, 0));

            var send = SendMessagesForeverWithSemaphoreAsync(connection);
            var receive = ReceiveMessagesForeverWithSemaphoreAsync(connection);

            await Task.WhenAll(new Task[] { send, receive });
        }

        public static async Task CleanupDictionariesWithSemaphoreAsync(TimeSpan timeSpan)
        {
            while (true)
            {
                await Task.Delay(timeSpan);
                await slimShady3.WaitAsync();

                var count = 0;
                var listOfItemsToRemove = models3.Where(x => x.Key.IsClosed).ToArray();
                foreach (var kvp in listOfItemsToRemove)
                {
                    models3.Remove(kvp.Key);
                    count++;
                }
                await Console.Out.WriteLineAsync($"Dead channels removed: {count}");

                slimShady3.Release();
            }
        }

        public static async Task MaintenanceWithSemaphoreAsync(TimeSpan timeSpan)
        {
            while (true)
            {
                await Task.Delay(timeSpan);
                await slimShady3.WaitAsync();

                double previousBytes = GC.GetTotalMemory(false);
                GC.AddMemoryPressure(Environment.SystemPageSize);
                GC.WaitForPendingFinalizers();
                GC.Collect();

                double totalBytes = (previousBytes - GC.GetTotalMemory(true)) / 1_000_000.0;
                await Console.Out.WriteLineAsync($"Maintenance finished Memory Reclaimed: {totalBytes}MB");

                slimShady3.Release();
            }
        }

        public static async Task SendMessagesForeverWithSemaphoreAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var sendMessages1 = SendMessageSemaphoreAsync(connection, counter++);
                var sendMessages2 = SendMessageSemaphoreAsync(connection, counter++);
                var sendMessages3 = SendMessageSemaphoreAsync(connection, counter++);
                var sendMessages4 = SendMessageSemaphoreAsync(connection, counter++);
                var sendMessages5 = SendMessageSemaphoreAsync(connection, counter++);

                await Task.WhenAll(new Task[] { sendMessages1, sendMessages2, sendMessages3, sendMessages4, sendMessages5 });
            }
        }

        public static async Task ReceiveMessagesForeverWithSemaphoreAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var receiveMessages1 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages2 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages3 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages4 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages5 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages6 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages7 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages8 = ReceiveMessagesSemaphoreAsync(connection, counter++);

                await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
            }
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

        public static async Task ReceiveMessagesSemaphoreAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            await slimShady3.WaitAsync();
            models3.Add(channel, counter);
            slimShady3.Release();

            await ReceiveMessageAsync(channel);

            channel.Dispose();
        }

        #endregion

        #region Memory Leak Work #4

        public static SemaphoreSlim slimShady4 = new SemaphoreSlim(1, 1);
        public static Task cleanupTask4;
        public static Task maintenanceTask4;
        public static Dictionary<IModel, int> models4 = new Dictionary<IModel, int>();

        public static async Task RunMemoryLeakFixAttempFourAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory, "SemaphoreTestConnection");
            cleanupTask4 = CleanupDictionariesWithSemaphoreWithBytesAsync(new TimeSpan(0, 0, 20));
            maintenanceTask4 = MaintenanceWithSemaphoreWithBytesAsync(new TimeSpan(0, 1, 0));

            var send = SendMessagesForeverWithSemaphoreAndBytesAsync(connection);
            var receive = ReceiveMessagesForeverWithSemaphoreWithBytesAsync(connection);

            await Task.WhenAll(new Task[] { send, receive });
        }

        public static async Task CleanupDictionariesWithSemaphoreWithBytesAsync(TimeSpan timeSpan)
        {
            while (true)
            {
                await Task.Delay(timeSpan);
                await slimShady4.WaitAsync();

                var count = 0;
                var listOfItemsToRemove = models4.Where(x => x.Key.IsClosed).ToArray();
                foreach (var kvp in listOfItemsToRemove)
                {
                    models4.Remove(kvp.Key);
                    count++;
                }
                await Console.Out.WriteLineAsync($"Dead channels removed: {count}");

                slimShady4.Release();
            }
        }

        public static async Task MaintenanceWithSemaphoreWithBytesAsync(TimeSpan timeSpan)
        {
            while (true)
            {
                await Task.Delay(timeSpan);
                await slimShady4.WaitAsync();

                double previousBytes = GC.GetTotalMemory(false);
                GC.AddMemoryPressure(Environment.SystemPageSize);
                GC.WaitForPendingFinalizers();
                GC.Collect();

                double totalBytes = (previousBytes - GC.GetTotalMemory(true)) / 1_000_000.0;
                await Console.Out.WriteLineAsync($"Maintenance finished Memory Reclaimed: {totalBytes}MB");
                await Task.Delay(100); // Pause all work temporarily - Allows CLR breathing room.

                slimShady4.Release();
            }
        }

        public static async Task SendMessagesForeverWithSemaphoreAndBytesAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var sendMessages1 = SendMessageSemaphoreWithBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages2 = SendMessageSemaphoreWithBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages3 = SendMessageSemaphoreWithBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages4 = SendMessageSemaphoreWithBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages5 = SendMessageSemaphoreWithBytesAsync(connection, counter++, await GetRandomByteArray());

                await Task.WhenAll(new Task[] { sendMessages1, sendMessages2, sendMessages3, sendMessages4, sendMessages5 });
            }
        }

        public static async Task ReceiveMessagesForeverWithSemaphoreWithBytesAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var receiveMessages1 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages2 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages3 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages4 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages5 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages6 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages7 = ReceiveMessagesSemaphoreAsync(connection, counter++);
                var receiveMessages8 = ReceiveMessagesSemaphoreAsync(connection, counter++);

                await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
            }
        }

        public static async Task SendMessageSemaphoreWithBytesAsync(IConnection connection, int counter, byte[] bytes)
        {
            var channel = await CreateChannel(connection);

            await slimShady4.WaitAsync();
            models4.Add(channel, counter);
            slimShady4.Release();

            await Task.Delay(25); // Simulate network connectivity
            await SendMessageWithBytesAsync(channel, counter, bytes);

            channel.Dispose();
        }

        public static async Task ReceiveMessagesSemaphoreWithBytesAsync(IConnection connection, int counter)
        {
            var channel = await CreateChannel(connection);

            await slimShady4.WaitAsync();
            models4.Add(channel, counter);
            slimShady4.Release();

            await Task.Delay(25); // Simulate network connectivity
            await ReceiveMessageAsync(channel);

            //await Console.Out.WriteLineAsync($"Iteration {counter} models Count: {models3.Count}");

            channel.Dispose();
        }

        #endregion

        #region Random Data Generation

        private static Random rand = new Random();

        private static uint x;
        private static uint y;
        private static uint z;
        private static uint w;

        public static async Task<byte[]> GetRandomByteArray()
        {
            var bytes = new byte[10000];

            x = (uint)rand.Next(0, 1000);
            y = (uint)rand.Next(0, 1000);
            z = (uint)rand.Next(0, 1000);
            w = (uint)rand.Next(0, 1000);
            await FillBuffer(bytes, 0, 10000);

            return bytes;
        }

        #region Performance XORSHIFT

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
