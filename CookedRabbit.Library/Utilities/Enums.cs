﻿using System;
using System.ComponentModel;
using System.Linq;

namespace CookedRabbit.Library.Utilities
{
    /// <summary>
    /// Cooked Rabbit Enumerations.
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Extension method of getting the Description value to string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Description(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Any() ? ((DescriptionAttribute)attributes.ElementAt(0)).Description : "Description Not Found";
        }

        /// <summary>
        /// Allows for quickling setting ContentEncoding for RabbitMQ IBasicProperties.
        /// </summary>
        public enum ContentEncoding
        {
            /// <summary>
            /// ContentEncoding.Gzip
            /// </summary>
            [Description("gzip")]
            Gzip,
            /// <summary>
            /// ContentEncoding.Br
            /// </summary>
            [Description("br")]
            Brotli,
            /// <summary>
            /// ContentEncoding.Compress
            /// </summary>
            [Description("compress")]
            Compress,
            /// <summary>
            /// ContentEncoding.Deflate
            /// </summary>
            [Description("deflate")]
            Deflate,
            /// <summary>
            /// ContentEncoding.Binary
            /// </summary>
            [Description("binary")]
            Binary,
            /// <summary>
            /// ContentEncoding.Base64
            /// </summary>
            [Description("base64")]
            Base64,
        }

        /// <summary>
        /// Allows for quickling setting ContentType for RabbitMQ IBasicProperties.
        /// </summary>
        public enum ContentType
        {
            /// <summary>
            /// ContentType.Javascript
            /// </summary>
            [Description("application/javascript;")]
            Javascript,
            /// <summary>
            /// ContentType.Json
            /// </summary>
            [Description("application/json;")]
            Json,
            /// <summary>
            /// ContentType.Urlencoded
            /// </summary>
            [Description("application/x-www-form-urlencoded;")]
            Urlencoded,
            /// <summary>
            /// ContentType.Xml
            /// </summary>
            [Description("application/xml;")]
            Xml,
            /// <summary>
            /// ContentType.Zip
            /// </summary>
            [Description("application/zip;")]
            Zip,
            /// <summary>
            /// ContentType.Pdf
            /// </summary>
            [Description("application/pdf;")]
            Pdf,
            /// <summary>
            /// ContentType.Sql
            /// </summary>
            [Description("application/sql;")]
            Sql,
            /// <summary>
            /// ContentType.Graphql
            /// </summary>
            [Description("application/graphql;")]
            Graphql,
            /// <summary>
            /// ContentType.Ldjson
            /// </summary>
            [Description("application/ld+json;")]
            Ldjson,
            /// <summary>
            /// ContentType.Msword
            /// </summary>
            [Description("application/msword(.doc);")]
            Msword,
            /// <summary>
            /// ContentType.Openword
            /// </summary>
            [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document(.docx);")]
            Openword,
            /// <summary>
            /// ContentType.Excel
            /// </summary>
            [Description("application/vnd.ms-excel(.xls);")]
            Excel,
            /// <summary>
            /// ContentType.Openexcel
            /// </summary>
            [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet(.xlsx);")]
            Openexcel,
            /// <summary>
            /// ContentType.Powerpoint
            /// </summary>
            [Description("application/vnd.ms-powerpoint(.ppt);")]
            Powerpoint,
            /// <summary>
            /// ContentType.Openpowerpoint
            /// </summary>
            [Description("application/vnd.openxmlformats-officedocument.presentationml.presentation(.pptx);")]
            Openpowerpoint,
            /// <summary>
            /// ContentType.Opendocument
            /// </summary>
            [Description("application/vnd.oasis.opendocument.text(.odt);")]
            Opendocument,
            /// <summary>
            /// ContentType.Audiompeg
            /// </summary>
            [Description("audio/mpeg;")]
            Audiompeg,
            /// <summary>
            /// ContentType.Audiovorbis
            /// </summary>
            [Description("audio/vorbis;")]
            Audiovorbis,
            /// <summary>
            /// ContentType.Multiformdata
            /// </summary>
            [Description("multipart/form-data;")]
            Multiformdata,
            /// <summary>
            /// ContentType.Textcss
            /// </summary>
            [Description("text/css;")]
            Textcss,
            /// <summary>
            /// ContentType.Texthtml
            /// </summary>
            [Description("text/html;")]
            Texthtml,
            /// <summary>
            /// ContentType.Textcsv
            /// </summary>
            [Description("text/csv;")]
            Textcsv,
            /// <summary>
            /// ContentType.Textplain
            /// </summary>
            [Description("text/plain;")]
            Textplain,
            /// <summary>
            /// ContentType.Png
            /// </summary>
            [Description("image/png;")]
            Png,
            /// <summary>
            /// ContentType.Jpeg
            /// </summary>
            [Description("image/jpeg;")]
            Jpeg,
            /// <summary>
            /// ContentType.Gif
            /// </summary>
            [Description("image/gif;")]
            Gif
        }

        /// <summary>
        /// Allows for quickling combining Charset with ContentType for RabbitMQ IBasicProperties.
        /// </summary>
        public enum Charset
        {
            /// <summary>
            /// Charset.Utf8
            /// </summary>
            [Description("charset=utf-8")]
            Utf8,
            /// <summary>
            /// Charset.Utf16
            /// </summary>
            [Description("charset=utf-16")]
            Utf16,
            /// <summary>
            /// Charset.Utf32
            /// </summary>
            [Description("charset=utf-32")]
            Utf32,
        }

        /// <summary>
        /// Help specify the RabbitMQ exchange you are going to create.
        /// </summary>
        public enum ExchangeType
        {
            /// <summary>
            /// RabbitMQ Exchange type called Direct.
            /// </summary>
            [Description("direct")]
            Direct,
            /// <summary>
            /// RabbitMQ Exchange type called Fanout.
            /// </summary>
            [Description("fanout")]
            Fanout,
            /// <summary>
            /// RabbitMQ Exchange type called Topic.
            /// </summary>
            [Description("topic")]
            Topic,
            /// <summary>
            /// RabbitMQ Exchange type called Header.
            /// </summary>
            [Description("header")]
            Header
        }

        /// <summary>
        /// CompressionMethod helps specify the compression method desired.
        /// </summary>
        public enum CompressionMethod
        {
            /// <summary>
            /// Compression method using builtin .NET GzipStream.
            /// </summary>
            Gzip,
            /// <summary>
            /// Compression method using builtin .NET DeflateStream.
            /// </summary>
            Deflate,
            /// <summary>
            /// Compression method using LZ4NET.
            /// </summary>
            LZ4,
            /// <summary>
            /// Compression method using LZ4NET wrap/unwrap in the LZ4Codec.
            /// </summary>
            LZ4Codec
        }

        /// <summary>
        /// SerializationMethod helps specify the serialization method desired.
        /// </summary>
        public enum SerializationMethod
        {
            /// <summary>
            /// Serialize object as a string using Utf8Json.
            /// </summary>
            JsonString,
            /// <summary>
            /// Serialize object as Utf8Json.
            /// </summary>
            Utf8Json,
            /// <summary>
            /// Serialize object as ZeroFormat.
            /// </summary>
            ZeroFormat
        }

        public enum RabbitApiTarget
        {
            [Description("connections")]
            Connections
        }
    }
}
