using RabbitMQ.Util;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RabbitMQ.Client.Impl
{
    public class MethodArgumentWriter
    {
        private byte m_bitAccumulator;
        private int m_bitMask;
        private bool m_needBitFlush;

        public MethodArgumentWriter(NetworkBinaryWriter writer)
        {
            BaseWriter = writer;
            if (!BaseWriter.BaseStream.CanSeek)
            {
                //FIXME: Consider throwing System.IO.IOException
                // with message indicating that the specified writer does not support Seeking

                // Only really a problem if we try to write a table,
                // but complain anyway. See WireFormatting.WriteTable
                throw new NotSupportedException("Cannot write method arguments to non-positionable stream");
            }
            ResetBitAccumulator();
        }

        public NetworkBinaryWriter BaseWriter { get; private set; }

        public void Flush()
        {
            BitFlush();
            BaseWriter.Flush();
        }

        public void WriteBit(bool val)
        {
            if (m_bitMask > 0x80)
            {
                BitFlush();
            }
            if (val)
            {
                // The cast below is safe, because the combination of
                // the test against 0x80 above, and the action of
                // BitFlush(), causes m_bitMask never to exceed 0x80
                // at the point the following statement executes.
                m_bitAccumulator = (byte)(m_bitAccumulator | (byte)m_bitMask);
            }
            m_bitMask = m_bitMask << 1;
            m_needBitFlush = true;
        }

        public void WriteContent(byte[] val)
        {
            throw new NotSupportedException("WriteContent should not be called");
        }

        public void WriteLong(uint val)
        {
            BitFlush();
            WireFormatting.WriteLong(BaseWriter, val);
        }

        public void WriteLonglong(ulong val)
        {
            BitFlush();
            WireFormatting.WriteLonglong(BaseWriter, val);
        }

        public void WriteLongstr(byte[] val)
        {
            BitFlush();
            WireFormatting.WriteLongstr(BaseWriter, val);
        }

        public void WriteOctet(byte val)
        {
            BitFlush();
            WireFormatting.WriteOctet(BaseWriter, val);
        }

        public void WriteShort(ushort val)
        {
            BitFlush();
            WireFormatting.WriteShort(BaseWriter, val);
        }

        public void WriteShortstr(string val)
        {
            BitFlush();
            WireFormatting.WriteShortstr(BaseWriter, val);
        }

        public void WriteTable(IDictionary val)
        {
            BitFlush();
            WireFormatting.WriteTable(BaseWriter, val);
        }

        public void WriteTable(IDictionary<string, object> val)
        {
            BitFlush();
            WireFormatting.WriteTable(BaseWriter, val);
        }

        public void WriteTimestamp(AmqpTimestamp val)
        {
            BitFlush();
            WireFormatting.WriteTimestamp(BaseWriter, val);
        }

        private void BitFlush()
        {
            if (m_needBitFlush)
            {
                BaseWriter.Write(m_bitAccumulator);
                ResetBitAccumulator();
            }
        }

        private void ResetBitAccumulator()
        {
            m_needBitFlush = false;
            m_bitAccumulator = 0;
            m_bitMask = 1;
        }

        // TODO: Consider using NotImplementedException (?)
        // This is a completely bizarre consequence of the way the
        // Message.Transfer method is marked up in the XML spec.
    }
}
