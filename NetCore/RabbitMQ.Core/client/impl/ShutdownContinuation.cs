using System;
using RabbitMQ.Util;

namespace RabbitMQ.Client.Impl
{
    public class ShutdownContinuation
    {
        public readonly BlockingCell m_cell = new BlockingCell();

        // You will note there are two practically identical overloads
        // of OnShutdown() here. This is because Microsoft's C#
        // compilers do not consistently support the Liskov
        // substitutability principle. When I use
        // OnShutdown(object,ShutdownEventArgs), the compilers
        // complain that OnShutdown can't be placed into a
        // ConnectionShutdownEventHandler because object doesn't
        // "match" IConnection, even though there's no context in
        // which the program could Go Wrong were it to accept the
        // code. The same problem appears for
        // ModelShutdownEventHandler. The .NET 1.1 compiler complains
        // about these two cases, and the .NET 2.0 compiler does not -
        // presumably they improved the type checker with the new
        // release of the compiler.

        public virtual void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            m_cell.Value = reason;
        }

        public virtual void OnModelShutdown(IModel sender, ShutdownEventArgs reason)
        {
            m_cell.Value = reason;
        }

        public virtual ShutdownEventArgs Wait()
        {
            return (ShutdownEventArgs)m_cell.Value;
        }

        public ShutdownEventArgs Wait(TimeSpan timeout)
        {
            return (ShutdownEventArgs)m_cell.GetValue(timeout);
        }
    }
}
