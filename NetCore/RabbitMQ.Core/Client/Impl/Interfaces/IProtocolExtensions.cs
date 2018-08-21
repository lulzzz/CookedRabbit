using RabbitMQ.Client.Impl;
using System;
using System.Net.Sockets;

namespace RabbitMQ.Client.Framing.Impl
{
    public static class IProtocolExtensions
    {
        public static IFrameHandler CreateFrameHandler(
            this IProtocol protocol,
            AmqpTcpEndpoint endpoint,
            Func<AddressFamily, ITcpClient> socketFactory,
            int connectionTimeout,
            int readTimeout,
            int writeTimeout)
        {
            return new SocketFrameHandler(endpoint, socketFactory,
                connectionTimeout, readTimeout, writeTimeout);
        }
    }
}