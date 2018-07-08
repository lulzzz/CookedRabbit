using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Utf8Json;
using ZeroFormatter;
using static CookedRabbit.Core.Library.Utilities.Enums;

namespace CookedRabbit.Core.Library.Utilities
{
    /// <summary>
    /// CookedRabbit Utility class to help serialize objects.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// A high performing New genericc object initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static class New<T>
        {
            /// <summary>
            /// Return a new GenericObject and cache the instantiation.
            /// </summary>
            public static readonly Func<T> Instance = Creator();

            private static Func<T> Creator()
            {
                Type t = typeof(T);
                if (t == typeof(string))
                    return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

                if (t.HasDefaultConstructor())
                    return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

                return () => (T)FormatterServices.GetUninitializedObject(t);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        /// Take an object of type T and serialize to a byte[].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message to be serialized.</param>
        /// <param name="serializationMethod">Specifies the serialization method.</param>
        /// <returns></returns>
        public static async Task<byte[]> SerializeAsync<T>(T message, SerializationMethod serializationMethod)
        {
            byte[] output = null;

            switch (serializationMethod)
            {
                case SerializationMethod.Utf8Json:
                    output = await SerializeAsUtf8JsonFormatAsync(message);
                    break;
                case SerializationMethod.ZeroFormat:
                    output = await SerializeAsZeroFormatAsync(message);
                    break;
                default: break;
            }

            return output;
        }

        /// <summary>
        /// Deserialize byte[] into an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="serializationMethod"></param>
        /// <returns></returns>
        public static async Task<T> DeserializeAsync<T>(byte[] input, SerializationMethod serializationMethod)
        {
            switch (serializationMethod)
            {
                case SerializationMethod.Utf8Json:
                    return await DeserializeAsUtf8JsonFormatAsync<T>(input);
                case SerializationMethod.ZeroFormat:
                    return await DeserializeAsZeroFormatAsync<T>(input);
                default:
                    return New<T>.Instance();
            }
        }

        /// <summary>
        /// Serialize an object that is ZeroFormattable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns>A byte[]</returns>
        public static async Task<byte[]> SerializeAsZeroFormatAsync<T>(T message)
        {
            return await Task.Run(() => ZeroFormatterSerializer.Serialize(message));
        }

        /// <summary>
        /// Deserialize an object that is ZeroFormattable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns>An object of type T</returns>
        public static async Task<T> DeserializeAsZeroFormatAsync<T>(byte[] input)
        {
            return await Task.Run(() => ZeroFormatterSerializer.Deserialize<T>(input));
        }

        /// <summary>
        /// Serialize an object with Utf8Json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns>A byte[]</returns>
        public static async Task<byte[]> SerializeAsUtf8JsonFormatAsync<T>(T message)
        {
            return await Task.Run(() => JsonSerializer.Serialize(message));
        }

        /// <summary>
        /// Deserialize an object with Utf8Json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns>An object of type T</returns>
        public static async Task<T> DeserializeAsUtf8JsonFormatAsync<T>(byte[] input)
        {
            return await Task.Run(() => JsonSerializer.Deserialize<T>(input));
        }
    }
}
