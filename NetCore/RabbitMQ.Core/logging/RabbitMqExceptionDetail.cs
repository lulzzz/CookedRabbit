
namespace RabbitMQ.Client.Logging
{
    using System;
    using System.Collections.Generic;
#if NET451
    using Microsoft.Diagnostics.Tracing;
#else
    using System.Diagnostics.Tracing;
#endif

    [EventData]
    public class RabbitMqExceptionDetail
    {
        public RabbitMqExceptionDetail(Exception ex)
        {
            this.Type = ex.GetType().FullName;
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;
            if(ex.InnerException != null)
            {
                this.InnerException = ex.InnerException.ToString();
            }
        }

        public RabbitMqExceptionDetail(IDictionary<string, object> ex)
        {
            this.Type = ex["Type"].ToString();
            this.Message = ex["Message"].ToString();
            this.StackTrace = ex["StackTrace"].ToString();
            object inner;
            if(ex.TryGetValue("InnerException", out inner))
            {
                this.InnerException = inner.ToString();
            }
        }

        public string Type { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }
        public string InnerException { get; private set; }

        public override string ToString()
        {
            return String.Format("Exception: {0}\r\n{1}\r\n\r\n{2}\r\nInnerException:\r\n{3}", Type, Message, StackTrace, InnerException);
        }
    }
}
