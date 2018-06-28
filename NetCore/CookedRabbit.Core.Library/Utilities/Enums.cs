using System;
using System.ComponentModel;
using System.Linq;

namespace CookedRabbit.Core.Library.Utilities
{
    public static class Enums
    {
        public static string Description(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Any() ? ((DescriptionAttribute)attributes.ElementAt(0)).Description : "Description Not Found";
        }

        public enum ContentEncoding
        {
            [Description("gzip")]
            Gzip,
            [Description("br")]
            Brotli,
            [Description("compress")]
            Compress,
            [Description("deflate")]
            Deflate,
            [Description("binary")]
            Binary,
            [Description("base64")]
            Base64,
        }

        public enum ContentType
        {
            [Description("application/javascript;")]
            Javascript,
            [Description("application/json;")]
            Json,
            [Description("application/x-www-form-urlencoded;")]
            Urlencoded,
            [Description("application/xml;")]
            Xml,
            [Description("application/zip;")]
            Zip,
            [Description("application/pdf;")]
            Pdf,
            [Description("application/sql;")]
            Sql,
            [Description("application/graphql;")]
            Graphql,
            [Description("application/ld+json;")]
            Ldjson,
            [Description("application/msword(.doc);")]
            Msword,
            [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document(.docx);")]
            Openword,
            [Description("application/vnd.ms-excel(.xls);")]
            Excel,
            [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet(.xlsx);")]
            Openexcel,
            [Description("application/vnd.ms-powerpoint(.ppt);")]
            Powerpoint,
            [Description("application/vnd.openxmlformats-officedocument.presentationml.presentation(.pptx);")]
            Openpowerpoint,
            [Description("application/vnd.oasis.opendocument.text(.odt);")]
            Opendocument,
            [Description("audio/mpeg;")]
            Audiompeg,
            [Description("audio/vorbis;")]
            Audiovorbis,
            [Description("multipart/form-data;")]
            Multiformdata,
            [Description("text/css;")]
            Textcss,
            [Description("text/html;")]
            Texthtml,
            [Description("text/csv;")]
            Textcsv,
            [Description("text/plain;")]
            Textplain,
            [Description("image/png;")]
            Png,
            [Description("image/jpeg;")]
            Jpeg,
            [Description("image/gif;")]
            Gif
        }

        public enum Charset
        {
            [Description("charset=utf-8")]
            utf8,
            [Description("charset=utf-16")]
            utf16,
            [Description("charset=utf-32")]
            utf32,
        }
    }
}
