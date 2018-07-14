using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Utilities
{
    /// <summary>
    /// Static class for generating filler (random) data for users and Tests.
    /// </summary>
    public static class RandomData
    {
        private static Random rand = new Random();
        private static uint x;
        private static uint y;
        private static uint z;
        private static uint w;

        /// <summary>
        /// Create a list of byte[] (default random data of 1KB each).
        /// </summary>
        /// <param name="payloadCount">The number of byte[] to create.</param>
        /// <param name="payloadSizeInBytes">The size of random bytes per byte[].</param>
        /// <returns>List of byte[] random bytes.</returns>
        public static async Task<List<byte[]>> CreatePayloadsAsync(int payloadCount, int payloadSizeInBytes = 1000)
        {
            if (payloadSizeInBytes < 10) throw new ArgumentException($"Argument {nameof(payloadSizeInBytes)} can't be less than 10 bytes.");

            var byteList = new List<byte[]>();

            for (int i = 0; i < payloadCount; i++)
            {
                byteList.Add(await GetRandomByteArray(payloadSizeInBytes));
            }

            return byteList;
        }

        /// <summary>
        /// Create a byte[] filled with user specified number of random bytes.
        /// </summary>
        /// <param name="sizeInBytes">Specifies how many random bytes to create, defaults to 10,000.</param>
        /// <returns></returns>
        public static async Task<byte[]> GetRandomByteArray(int sizeInBytes = 10000)
        {
            var bytes = new byte[sizeInBytes];

            x = (uint)rand.Next(0, 1000);
            y = (uint)rand.Next(0, 1000);
            z = (uint)rand.Next(0, 1000);
            w = (uint)rand.Next(0, 1000);
            await FillBuffer(bytes, 0, sizeInBytes);

            return bytes;
        }

        // Simple XorShift
        private static Task FillBuffer(byte[] buffer, int offset, int offsetEnd)
        {
            while (offset < offsetEnd)
            {
                int mask = 0xFF;
                uint t = x ^ (x << 11);
                x = y; y = z; z = w;
                w = w ^ (w >> 19) ^ (t ^ (t >> 8));
                buffer[offset++] = (byte)(w & mask);
                buffer[offset++] = (byte)((w >> 8) & mask);
                buffer[offset++] = (byte)((w >> 16) & mask);
                buffer[offset++] = (byte)((w >> 24) & mask);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Compare two byte[] and identify if they are equal.
        /// </summary>
        /// <param name="input">Input byte[]</param>
        /// <param name="comparator">Comparator byte[]</param>
        /// <returns>True or false based on equality.</returns>
        public static Task<bool> ByteArrayCompare(ReadOnlySpan<byte> input, ReadOnlySpan<byte> comparator)
        {
            return Task.FromResult(input.SequenceEqual(comparator));
        }

        private const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()_-+=";

        /// <summary>
        /// Random asynchronous string generator.
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static async Task<string> RandomStringAsync(int minLength, int maxLength)
        {
            var t = await Task.Run(() =>
            {
                char[] chars = new char[maxLength];
                int setLength = AllowedChars.Length;

                int length = rand.Next(minLength, maxLength + 1);

                for (int i = 0; i < length; ++i)
                {
                    chars[i] = AllowedChars[rand.Next(setLength)];
                }

                return new string(chars, 0, length);
            });

            return t;
        }

        /// <summary>
        /// Random string generator.
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string RandomString(int minLength, int maxLength)
        {
            char[] chars = new char[maxLength];
            int setLength = AllowedChars.Length;

            int length = rand.Next(minLength, maxLength + 1);

            for (int i = 0; i < length; ++i)
            {
                chars[i] = AllowedChars[rand.Next(setLength)];
            }

            return new string(chars, 0, length);
        }
    }
}
