
namespace RabbitMQ.Client.Logging
{
    using System;
    using System.Collections.Generic;
#if NET451
    using Microsoft.Diagnostics.Tracing;
#else
    using System.Diagnostics.Tracing;
#endif

    public sealed class RabbitMqConsoleEventListener : EventListener, IDisposable
    {
        public RabbitMqConsoleEventListener()
        {
            this.EnableEvents(RabbitMqClientEventSource.Log, EventLevel.Informational, RabbitMqClientEventSource.Keywords.Log);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            foreach(var pl in eventData.Payload)
            {
                if (pl is IDictionary<string, object> dict)
                {
                    var rex = new RabbitMqExceptionDetail(dict);
                    Console.WriteLine("{0}: {1}", eventData.Level, rex.ToString());
                }
                else
                {
                    Console.WriteLine("{0}: {1}", eventData.Level, pl.ToString());
                }
            }
        }

        public override void Dispose()
        {
            this.DisableEvents(RabbitMqClientEventSource.Log);
        }
    }
}