using System;
using System.Threading.Tasks;
using static CookedRabbit.BatchSendExamples;
using static CookedRabbit.MemoryLeakExamples;
using static CookedRabbit.ChannelExamples;
using static CookedRabbit.DemoHelper;
using System.Diagnostics;

namespace CookedRabbit
{
    public class Program
    {
        private static readonly string DefaultErrorMessage = "Unhandled Error Occurred. No details are shown because something went horribly wrong and this is a catch all.";

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
            // Fun Error Handling
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleUnhandledException);
            TaskScheduler.UnobservedTaskException += HandleUnobservedTaskExceptionHandler;

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
            //await RunMemoryLeakFixAttemptSixAsync(); // ConcurrentDictionary

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

            // Testing out various channel creation methods
            //await RunManualTransientChannelTestAsync();
            //await RunPoolChannelTestAsync();

            // All Together Using RabbitService
            //await RunRabbitServicePoolChannelTestAsync();
            //await RunRabbitServiceAccuracyTestAsync();

            // Adding a Manual Ack Implementation
            await RunRabbitServiceDelayAckTestAsync();

            await Console.In.ReadLineAsync();
        }

        private static async void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = DefaultErrorMessage;

            if (e.ExceptionObject is Exception uex)
            {
                uex.Demystify();
                message = "Congratulations, you have broken this program like no other user before you!"
                    + $"\n\nException : {uex.Message}\n\nStack : {uex.StackTrace}";
            }
            else if (sender is Exception ex)
            {
                ex.Demystify();
                message = "Congratulations, you have broken this program like no other user before you!"
                    + $"\n\nException : {ex.Message}\n\nStack : {ex.StackTrace}";
            }

            await Console.Out.WriteAsync(message);
        }

        private static async void HandleUnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var message = DefaultErrorMessage;
            e?.SetObserved(); // Stops the process from terminating.

            if (e.Exception is Exception tuex)
            {
                tuex.Demystify();
                message = "Congratulations, you have broken this program like no other user before you!"
                    + $"\n\nException : {tuex.Message}\n\nStack : {tuex.StackTrace}";
            }
            else if (sender is Exception ex)
            {
                ex.Demystify();
                message = "Congratulations, you have broken this program like no other user before you!"
                    + $"\n\nException : {ex.Message}\n\nStack : {ex.StackTrace}";
            }

            await Console.Out.WriteAsync(message);
        }
    }
}
