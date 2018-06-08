using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            await RunBasicSendReceiveExampleAsync();

            await RunMemoryLeakAsync();

            await RunMemoryLeakMadeWorseAsync();

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
            return Task.FromResult(new ConnectionFactory() { HostName = "localhost" });
        }

        public static Task<IConnection> CreateConnection(IConnectionFactory connectionFactory)
        {
            return Task.FromResult(connectionFactory.CreateConnection());
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

        public static async Task<BasicGetResult> ReceiveMessageAsync(IModel channel)
        {
            return await Task.Run(() => { return channel.BasicGet(queue: "001", true); });
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

            foreach(var kvp in ChannelSequences)
            {
                await Console.Out.WriteLineAsync($"WTF AM I: {kvp.Key.GetType()} AND WHAT DO I HOLD: {kvp.Key}");
            }

            for(int i = 0; i < ChannelSequences.Count; i++)
            {
                try
                {
                    var channel = ChannelSequences.ElementAt(i).Key;
                    await SendMessageAsync(channel, i + 1000);
                }
                catch(RabbitMQ.Client.Exceptions.AlreadyClosedException ace)
                {
                    await Console.Out.WriteLineAsync($"AlreadyClosedException #{i.ToString("000")} Reason: {ace.ShutdownReason}");
                }
            }

            for(int i = 0; i < 5; i++)
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
    }
}
