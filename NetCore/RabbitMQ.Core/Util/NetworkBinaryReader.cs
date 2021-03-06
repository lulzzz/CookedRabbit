using System.IO;
using System.Text;

namespace RabbitMQ.Util
{
    /// <summary>
    /// Subclass of BinaryReader that reads integers etc in correct network order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Kludge to compensate for .NET's broken little-endian-only BinaryReader.
    /// Relies on BinaryReader always being little-endian.
    /// </para>
    /// </remarks>
    public class NetworkBinaryReader : BinaryReader
    {
        // Not particularly efficient. To be more efficient, we could
        // reuse BinaryReader's implementation details: m_buffer and
        // FillBuffer, if they weren't private
        // members. Private/protected claim yet another victim, film
        // at 11. (I could simply cut-n-paste all that good code from
        // BinaryReader, but two wrongs do not make a right)

        /// <summary>
        /// Construct a NetworkBinaryReader over the given input stream.
        /// </summary>
        public NetworkBinaryReader(Stream input) : base(input)
        {
        }

        /// <summary>
        /// Construct a NetworkBinaryReader over the given input
        /// stream, reading strings using the given encoding.
        /// </summary>
        public NetworkBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        ///<summary>Helper method for constructing a temporary
        ///BinaryReader over a byte[].</summary>
        public static BinaryReader TemporaryBinaryReader(byte[] bytes)
        {
            return new BinaryReader(new MemoryStream(bytes));
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override double ReadDouble()
        {
            byte[] bytes = ReadBytes(8);
            byte temp = bytes[0];
            bytes[0] = bytes[7];
            bytes[7] = temp;
            temp = bytes[1];
            bytes[1] = bytes[6];
            bytes[6] = temp;
            temp = bytes[2];
            bytes[2] = bytes[5];
            bytes[5] = temp;
            temp = bytes[3];
            bytes[3] = bytes[4];
            bytes[4] = temp;
            return TemporaryBinaryReader(bytes).ReadDouble();
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override short ReadInt16()
        {
            uint i = base.ReadUInt16();
            return (short)(((i & 0xFF00) >> 8) |
                           ((i & 0x00FF) << 8));
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override int ReadInt32()
        {
            uint i = base.ReadUInt32();
            return (int)(((i & 0xFF000000) >> 24) |
                         ((i & 0x00FF0000) >> 8) |
                         ((i & 0x0000FF00) << 8) |
                         ((i & 0x000000FF) << 24));
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override long ReadInt64()
        {
            ulong i = base.ReadUInt64();
            return (long)(((i & 0xFF00000000000000) >> 56) |
                          ((i & 0x00FF000000000000) >> 40) |
                          ((i & 0x0000FF0000000000) >> 24) |
                          ((i & 0x000000FF00000000) >> 8) |
                          ((i & 0x00000000FF000000) << 8) |
                          ((i & 0x0000000000FF0000) << 24) |
                          ((i & 0x000000000000FF00) << 40) |
                          ((i & 0x00000000000000FF) << 56));
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override float ReadSingle()
        {
            byte[] bytes = ReadBytes(4);
            byte temp = bytes[0];
            bytes[0] = bytes[3];
            bytes[3] = temp;
            temp = bytes[1];
            bytes[1] = bytes[2];
            bytes[2] = temp;
            return TemporaryBinaryReader(bytes).ReadSingle();
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override ushort ReadUInt16()
        {
            uint i = base.ReadUInt16();
            return (ushort)(((i & 0xFF00) >> 8) |
                            ((i & 0x00FF) << 8));
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override uint ReadUInt32()
        {
            uint i = base.ReadUInt32();
            return (((i & 0xFF000000) >> 24) |
                    ((i & 0x00FF0000) >> 8) |
                    ((i & 0x0000FF00) << 8) |
                    ((i & 0x000000FF) << 24));
        }

        /// <summary>
        /// Override BinaryReader's method for network-order.
        /// </summary>
        public override ulong ReadUInt64()
        {
            ulong i = base.ReadUInt64();
            return (((i & 0xFF00000000000000) >> 56) |
                    ((i & 0x00FF000000000000) >> 40) |
                    ((i & 0x0000FF0000000000) >> 24) |
                    ((i & 0x000000FF00000000) >> 8) |
                    ((i & 0x00000000FF000000) << 8) |
                    ((i & 0x0000000000FF0000) << 24) |
                    ((i & 0x000000000000FF00) << 40) |
                    ((i & 0x00000000000000FF) << 56));
        }
    }
}
