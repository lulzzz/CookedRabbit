using System.Collections.Generic;
using System.Net;

namespace RabbitMQ.Client.Impl
{
    public interface IFrameHandler
    {
        AmqpTcpEndpoint Endpoint { get; }

        EndPoint LocalEndPoint { get; }

        int LocalPort { get; }

        EndPoint RemoteEndPoint { get; }

        int RemotePort { get; }

        ///<summary>Socket read timeout, in milliseconds. Zero signals "infinity".</summary>
        int ReadTimeout { set; }

        ///<summary>Socket write timeout, in milliseconds. Zero signals "infinity".</summary>
        int WriteTimeout { set; }

        void Close();

        ///<summary>Read a frame from the underlying
        ///transport. Returns null if the read operation timed out
        ///(see Timeout property).</summary>
        InboundFrame ReadFrame();

        void SendHeader();

        void WriteFrame(OutboundFrame frame);

        void WriteFrameSet(IList<OutboundFrame> frames);
    }
}
