# CookedRabbit
Creating a simple RabbitMQ wrapper for dealing with channels and connection headaches. It also shows you  the natural evolution to common everyday problems with RabbitMQ implementations and how to avoid them.

Everything begins with the Demo client demonstrating MemoryLeaks. Storing IModels (RabbitMQ term for Channels) in container objects makes code prone to memory leaks, thus it is good to see how not to do things as well as how I do it. The examples in the CookedRabbit.Demo demonstrate very rudimentary usages of RabbitMQ. It's not supposed to be rocket science. The library is the simplification, removal, and abstraction of common usage code when wrapping RabbitMQ DotNetClient. It continues to add complexity and simplification at the same time in the RabbitBus & RabbitService.

Inspired to write my own much dumber RabbitMQ wrapper than RawRabbit (https://github.com/pardahlman/RawRabbit). The longterm goal is to be short and sweet, nothing more and nothing less. If you need a more thorough/advanced solution, I highly recommend checking out RawRabbit.

### Configuring RabbitMQ Server First (if running Local)
To run .Demo locally, have Erlang 20.3 and Server RabbitMQ v3.7.5 installed locally and running first.
Use the HTTP API management from RabbitMQ to verify communication is occurring.
The WarmupAsync() will create the queue '001' to work with, if it doesn't exist, and send/receive a test message.

### NetFramework
#### Solution requires Visual Studio 2017+, .NET 4.7.2 SDK installed, and using C# 7.2+ language features.

### NetCore
#### Solution requires Visual Studio Code, point to NetCore folder, and SDK NetCore 2.1.0

*Note: (NetCore runtime 2.1.1 seems buggy at this time)*

#### Library Topology At A Glance

    ║
    ║ Your Business Logic
    ║
    ╚══ » RabbbitBus() ═════════════════════════════════════════════════════════════╗
            ║                                                                       ║
            ║ - Exception Handling                                                  ║
            ║ - Circuit Breaker                                                     ║
            ║ - Abstraction                                                         ║
            ║                                                                       ║
            ╚══ » RabbitService(hostname, connectionname) ══════════════════════════╣
                    ║                                                               ║
                    ║ &RabbitChannelPool                                            ║
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
                    ╚══ » RabbitChannelPool(hostname, connectionname) ══════════════╣
                            ║                                                       ║
                            ║ &RabbitConnectionPool                                 ║
                            ║ + GetTransientChannel (non-Ackable)                   ║
                            ║ + GetTransientChannel (Ackable)                       ║
                            ║ + GetChannelPair from &ChannelPool (non-Ackable)      ║
                            ║ + GetChannelPair from &ChannelPool (ackable)          ║
                            ║ - Delay when Channels Are In Pool                     ║
                            ║ - In-Use Pool                                         ║
                            ║ - Replacing Console with Logger                       ║
                            ║ - throw ex                                            ║
                            ║ ! System For Dealing With Flagged Dead Channels       ║
                            ║                                                       ║
                            ╚══ » RabbitConnectionPool(hostname, connectionname) ═══╣
                                    ║                                               ║
                                    ║ &RabbitMQ ConnectionFactory                   ║
                                    ║ &ConnectionPool                               ║
                                    ╚═══════════════════════════════════════════════╝

Legend:  

    & Indicates mandatory/crucial internal object.  
    + Exists (or exists with future enhancements)  
    - Does not exist yet.  
    ! Important  
