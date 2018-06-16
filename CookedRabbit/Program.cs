using System;
using System.Threading.Tasks;
using static CookedRabbit.BatchSendExamples;
using static CookedRabbit.MemoryLeakExamples;
using static CookedRabbit.Helpers;

namespace CookedRabbit
{
    public class Program
    {
        //// Needed if not C# (7.1+)
        //public static void Main(string[] args)
        //{   //Rename other Main to MainAsync
        //    MainAsync(args).GetAwaiter().GetResult();
        //}

        // To Run, have Erlang 20.3 and Server RabbitMQ v3.7.5 installed locally
        // and running first. Use the HTTP API management from RabbitMQ to verify
        // communication is occurring.
        public static async Task Main(string[] args)
        {
            // Basic queue create, message send, message received.
            await WarmupAsync();

            //await RunMemoryLeakAsync();

            //await RunMemoryLeakMadeWorseAsync();

            //await RunMemoryLeakFixAttemptOneAsync();

            //await RunMemoryLeakFixAttemptTwoAsync();

            // Focus on this method to see high performance in concurrent threads using
            // a dictionary that is also being cleaned up without deadlocks or
            // exceptions.
            //await RunMemoryLeakFixAttemptThreeAsync(); // Semaphore
            await RunMemoryLeakFixAttemptSixAsync(); // ConcurrentDictionary

            // Focus on this method to see high IO usage in concurrent threads using
            // a dictionary that is also being cleaned up without deadlocks or
            // exceptions. Real data payloads and send/receives simulate network
            // communication times.
            //await RunMemoryLeakFixAttemptFourAsync(); // Semaphore
            //await RunMemoryLeakFixAttemptFiveAsync(); // ConcurrentDictionary

            // THESE Demonstrate how little memory is used by re-using a channel.
            //
            // Same as RunMemoryLeakFixAttemptSixAsync with a single channel instead of disposables.
            // Run Stress Test with Multi-Thread access to a Channel.
            // Channels shared across threads is not recommended but when compared to Memory Leak Work #6,
            // the performance is stellar.
            //await RunCrossThreadChannelsOneAsync();

            // Send messages in batches over a single channel.
            //await RunNonCrossThreadChannelsAsync();

            await Console.In.ReadLineAsync();
        }
    }
}
