using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CookedRabbit.Helpers;

namespace CookedRabbit
{
    public static class MemoryLeakExamples
    {
        #region Reproducing Memoryleak

        static readonly Dictionary<IModel, ConcurrentDictionary<ulong, TaskCompletionSource<ulong>>>
            ConfirmsDictionary = new Dictionary<IModel, ConcurrentDictionary<ulong, TaskCompletionSource<ulong>>>();

        static readonly ConcurrentDictionary<IModel, object> ChannelLocks = new ConcurrentDictionary<IModel, object>();

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

        public static async Task RunMemoryLeakFixAttemptOneAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            cleanupTask = CleanupDictionariesAsync(new TimeSpan(0, 0, 20));

            var sendMessage = SendMessageForeverAsync(connection);
            var receiveMessage = ReceiveMessageForeverAsync(connection);

            await Task.WhenAll(new Task[] { sendMessage, receiveMessage });
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

        #endregion

        #region Memory Leak Work #2

        public static async Task RunMemoryLeakFixAttemptTwoAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory);
            cleanupTask = CleanupDictionariesWithLocksAsync(new TimeSpan(0, 0, 20));

            var sendMessage = SendMessageForeverWithLocksAsync(connection);
            var receiveMessage = ReceiveMessageForeverWithLocksAsync(connection);

            await Task.WhenAll(new Task[] { sendMessage, receiveMessage });
        }

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

        #endregion

        #region Memory Leak Work #3 - Stress Performance Test With SemaphoreSlim

        public static async Task RunMemoryLeakFixAttemptThreeAsync()
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

                double totalBytes = (previousBytes - GC.GetTotalMemory(true)) / 1000000.0;
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

        #endregion

        #region Memory Leak Work #4 - Regular Dictionary (w/ SemaphoreSlim)

        public static async Task RunMemoryLeakFixAttemptFourAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory, "SemaphoreRealPayloadTestConnection");
            cleanupTask4 = CleanupDictionariesWithSemaphoreWithBytesAsync(new TimeSpan(0, 0, 20));
            //maintenanceTask4 = MaintenanceWithSemaphoreWithBytesAsync(new TimeSpan(0, 1, 0));

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

                double totalBytes = (previousBytes - GC.GetTotalMemory(true)) / 1000000.0;
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
                var receiveMessages1 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);
                var receiveMessages2 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);
                var receiveMessages3 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);
                var receiveMessages4 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);
                var receiveMessages5 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);
                var receiveMessages6 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);
                var receiveMessages7 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);
                var receiveMessages8 = ReceiveMessagesSemaphoreWithBytesAsync(connection, counter++);

                await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
            }
        }

        #endregion

        #region Memory Leak Work #5 - Concurrent Dictionary (No SemaphoreSlim)

        public static async Task RunMemoryLeakFixAttemptFiveAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory, "SemaphoreRealPayloadConcurrentTestConnection");
            cleanupTask5 = CleanupDictionariesWithConcurrentDictionaryWithBytesAsync(new TimeSpan(0, 0, 20));

            var send = SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(connection);
            var receive = ReceiveMessagesForeverWithConcurrentDictionaryWithBytesAsync(connection);

            await Task.WhenAll(new Task[] { send, receive });
        }

        public static async Task CleanupDictionariesWithConcurrentDictionaryWithBytesAsync(TimeSpan timeSpan)
        {
            while (true)
            {
                await Task.Delay(timeSpan);

                var count = 0;
                var listOfItemsToRemove = models6.Where(x => x.Key.IsClosed).ToArray();
                foreach (var key in listOfItemsToRemove)
                {
                    models5.Remove(key.Key);
                    count++;
                }
                await Console.Out.WriteLineAsync($"Dead channels removed: {count}");
            }
        }

        public static async Task SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var sendMessages1 = Helpers.SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages2 = Helpers.SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages3 = Helpers.SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages4 = Helpers.SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(connection, counter++, await GetRandomByteArray());
                var sendMessages5 = Helpers.SendMessagesForeverWithConcurrentDictionaryAndBytesAsync(connection, counter++, await GetRandomByteArray());

                await Task.WhenAll(new Task[] { sendMessages1, sendMessages2, sendMessages3, sendMessages4, sendMessages5 });
            }
        }

        public static async Task ReceiveMessagesForeverWithConcurrentDictionaryWithBytesAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var receiveMessages1 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);
                var receiveMessages2 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);
                var receiveMessages3 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);
                var receiveMessages4 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);
                var receiveMessages5 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);
                var receiveMessages6 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);
                var receiveMessages7 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);
                var receiveMessages8 = ReceiveMessagesConcurrentDictionaryWithBytesAsync(connection, counter++);

                await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
            }
        }

        #endregion

        #region Memory Leak Work #6 - Concurrent Dictionary (No SemaphoreSlim)

        public static async Task RunMemoryLeakFixAttemptSixAsync()
        {
            var connectionFactory = await CreateChannelFactoryAsync();
            var connection = await CreateConnection(connectionFactory, "SemaphoreRealPayloadConcurrentTestConnection");
            cleanupTask6 = CleanupDictionariesWithConcurrentDictionaryAsync(new TimeSpan(0, 0, 20));

            var send = SendMessagesForeverWithConcurrentDictionaryAsync(connection);
            var receive = ReceiveMessagesForeverWithConcurrentDictionaryAsync(connection);

            await Task.WhenAll(new Task[] { send, receive });
        }

        public static async Task CleanupDictionariesWithConcurrentDictionaryAsync(TimeSpan timeSpan)
        {
            while (true)
            {
                await Task.Delay(timeSpan);

                var count = 0;
                var listOfItemsToRemove = models6.Where(x => x.Key.IsClosed).ToArray();
                foreach (var key in listOfItemsToRemove)
                {
                    models6.Remove(key.Key);
                    count++;
                }
                await Console.Out.WriteLineAsync($"Dead channels removed: {count}");
            }
        }

        public static async Task SendMessagesForeverWithConcurrentDictionaryAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var sendMessages1 = Helpers.SendMessagesForeverWithConcurrentDictionaryAsync(connection, counter++);
                var sendMessages2 = Helpers.SendMessagesForeverWithConcurrentDictionaryAsync(connection, counter++);
                var sendMessages3 = Helpers.SendMessagesForeverWithConcurrentDictionaryAsync(connection, counter++);
                var sendMessages4 = Helpers.SendMessagesForeverWithConcurrentDictionaryAsync(connection, counter++);
                var sendMessages5 = Helpers.SendMessagesForeverWithConcurrentDictionaryAsync(connection, counter++);

                await Task.WhenAll(new Task[] { sendMessages1, sendMessages2, sendMessages3, sendMessages4, sendMessages5 });
            }
        }

        public static async Task ReceiveMessagesForeverWithConcurrentDictionaryAsync(IConnection connection)
        {
            int counter = 0;
            while (true)
            {
                var receiveMessages1 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);
                var receiveMessages2 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);
                var receiveMessages3 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);
                var receiveMessages4 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);
                var receiveMessages5 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);
                var receiveMessages6 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);
                var receiveMessages7 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);
                var receiveMessages8 = ReceiveMessagesConcurrentDictionaryAsync(connection, counter++);

                await Task.WhenAll(new Task[] { receiveMessages1, receiveMessages2, receiveMessages3, receiveMessages4, receiveMessages5, receiveMessages6, receiveMessages7, receiveMessages8 });
            }
        }

        #endregion
    }
}
