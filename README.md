# CookedRabbit ![build](https://ci.appveyor.com/api/projects/status/github/thyams/CookedRabbit?branch=master&svg=true)
Creating a simple RabbitMQ wrapper for dealing with channels and connection headaches. It also shows you  the natural evolution to common everyday problems with RabbitMQ implementations and how to avoid them.

Everything begins with the Demo client demonstrating MemoryLeaks. Storing IModels (RabbitMQ term for Channels) in container objects makes code prone to memory leaks, thus it is good to see how not to do things as well as how I do it. The examples in the CookedRabbit.Demo demonstrate very rudimentary usages of RabbitMQ. It's not supposed to be rocket science. The library is the simplification, removal, and abstraction of common usage code when wrapping RabbitMQ DotNetClient. It continues to add complexity and simplification at the same time in the RabbitBus & RabbitService.

Inspired to write my own much dumber RabbitMQ wrapper than RawRabbit (https://github.com/pardahlman/RawRabbit). The longterm goal is to be short and sweet, nothing more and nothing less. If you need a more thorough/advanced solution, I highly recommend checking out RawRabbit or EastyNetQ.

### Why use CookedRabbit?
Do or do not, I am not really bothered either way. One actual benefit to using CookedRabbit is that I will only keep it simple. I will also stay current with .Net Framework, NetCore, C#7.x+, and the RabbitMQ client. It is not my intention to let things lag behind Pivotal RabbitMQ or Microsoft for that matter.

Which leads me to the custom compiled RabbitMQ CookedRabbit uses:

    RabbitMQ Dotnet Client 5.1.0 (6/23/2018)

    Changes from Official Release
       All NuGets updated.
       NetFramework 4.5/4.5.1 -> 4.7.2
       NetStandard 1.5 -> 2.0
       NetCore 2.0 -> 2.1
       C# 7.3 (latest version)
       ApiGen re-compiled.
       Client compiled as x64 (for x86 CookedRabbit, have to go with the official NuGet from RabbitMQ)

### Configuring RabbitMQ Server First (if running Local)
To run .Demo locally, have Erlang 20.3 and Server RabbitMQ v3.7.5 installed locally and running first.
Use the HTTP API management from RabbitMQ to verify communication is occurring.
The WarmupAsync() will create the queue '001' to work with, if it doesn't exist, and send/receive a test message.

### NetFramework

    Solution requires Visual Studio 2017+, .NET 4.7.2 SDK installed, and using C# 7.2+ language features.

### NetCore

    Same or Visual Studio Code, opening up NetCore folder. NetCore 2.1.0 SDK also installed.

*Note: (NetCore runtime 2.1.1 seems buggy at this time)*

### Default Values Currently Hardcoded

Pools:  

~~Connections: 10~~  
~~ChannelPool Channels: 100 (AutoAck), 100 (ManualAck), Distributed Across Connections~~  

Connection Factory:  

~~Heartbeats: 15s~~  
~~MaxChannels: 1000 (per Connection)~~  
~~AutomaticRecoveryEnabled: true~~  
~~RecoverTopologyEnabled: true~~  
~~NetworkRecoveryInterval: 10s~~  

 PublishManyAsBatches:  

     BatchSize: 100

 Consumer:  

     BasicQos(0, prefetchSize, false)

#### Library Topology At A Glance

    ║
    ║ Your Business Logic
    ║
    ╠══ » RabbbitBus() ═════════════════════════════════════════════════════════════╗
    ║       ║                                                                       ║
    ║       ║ - Exception Handling                                                  ║
    ║       ║ - Circuit Breaker                                                     ║
    ║       ║ - Abstraction                                                         ║
    ║       ║                                                                       ║
    ╚════ » ╚══ » RabbitService ════════════════════════════════════════════════════╣
                    ║                                                               ║
                    ║ & RabbitChannelPool                                           ║
                    ║ + Flag Channel As Dead                                        ║
                    ║ + Return Channel To Pool (Finished Work)                      ║
                    ║ + Publish                                                     ║
                    ║ + PublishMany                                                 ║
                    ║ + PublishManyAsBatches                                        ║
                    ║ + Get                                                         ║
                    ║   + Returns As ValueTuple or AckableResult                    ║
                    ║ + GetMany                                                     ║
                    ║   + Returns As ValueTuple or AckableResult                    ║
                    ║ + CreateConsumer                                              ║
                    ║ - Replacing Console with Logger                               ║
                    ║ - throw ex                                                    ║
                    ║ ! Opinionated Throttling                                      ║
                    ║                                                               ║
                    ╚══ » RabbitChannelPool ════════════════════════════════════════╣
                            ║                                                       ║
                            ║ & RabbitConnectionPool                                ║
                            ║ + GetTransientChannel (non-Ackable)                   ║
                            ║ + GetTransientChannel (Ackable)                       ║
                            ║ + GetChannelPair from &ChannelPool (non-Ackable)      ║
                            ║ + GetChannelPair from &ChannelPool (ackable)          ║
                            ║ + Get Channel Delay (When All Channels Are In Use)    ║
                            ║ + In Use ChannelPair Pool                             ║
                            ║ + In Use Ack ChannelPair Pool                         ║
                            ║ + Return Channel to A Pool                            ║
                            ║ - Replacing Console with Logger                       ║
                            ║ - throw ex                                            ║
                            ║ ! System For Dealing With Flagged Dead Channels       ║
                            ║                                                       ║
                            ╚══ » RabbitConnectionPool ═════════════════════════════╣
                                    ║                                               ║
                                    ║ & RabbitMQ ConnectionFactory                  ║
                                    ║ & ConnectionPool                              ║
                                    ║ - System for Dealing with Flagged Connections ║
                                    ║                                               ║
                                    ╚═══════════════════════════════════════════════╝

Legend  

    & Indicates mandatory/crucial internal object.  
    + Exists (or exists with future enhancements)  
    - Does not exist yet.  
    ! Important  
