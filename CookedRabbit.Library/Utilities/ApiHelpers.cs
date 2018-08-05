using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CookedRabbit.Library.Utilities
{
    /// <summary>
    /// Class for helping communication with Rabbit API.
    /// </summary>
    public static class ApiHelpers
    {
        /// <summary>
        /// Creates a HttpRequestMessage.
        /// </summary>
        /// <param name="apiPath"></param>
        /// <param name="subPath"></param>
        /// <param name="httpMethod"></param>
        /// <returns>HttpRequestMessage for posting to Rabbit HTTP API.</returns>
        public static HttpRequestMessage CreateRequest(string apiPath, string subPath, HttpMethod httpMethod)
        {
            var apiUri = new Uri($"{apiPath}/{subPath}");
            return new HttpRequestMessage(httpMethod, apiUri);
        }

        /// <summary>
        /// Creates a HttpRequestMessage.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="portNumber"></param>
        /// <param name="path"></param>
        /// <param name="httpMethod"></param>
        /// <returns>HttpRequestMessage for posting to Rabbit HTTP API.</returns>
        public static HttpRequestMessage CreateRequest(string hostName, int portNumber, string path, HttpMethod httpMethod)
        {
            var uri = new Uri($"{hostName}:{portNumber}/api/{path}");
            return new HttpRequestMessage(httpMethod, uri);
        }

        /// <summary>
        /// Converts DateTime to Unix time.
        /// </summary>
        /// <param name="unixSeconds"></param>
        /// <param name="returnLocalTime"></param>
        /// <returns></returns>
        public static DateTime UnixSecondsToDateTime(long unixSeconds, bool returnLocalTime = false)
        {
            DateTime dateTime;

            if (returnLocalTime) { dateTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).LocalDateTime; }
            else { dateTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).DateTime; }

            return dateTime;
        }

        /// <summary>
        /// Creates a HttpClient with Network Credentials.
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static HttpClient CreateHttpClient(NetworkCredential credentials, TimeSpan timeout)
        {
            var httpClient = new HttpClient(new HttpClientHandler { Credentials = credentials })
            { Timeout = timeout };

            return httpClient;
        }

        /// <summary>
        /// Creates the Api base path string.
        /// </summary>
        /// <param name="useSsl"></param>
        /// <param name="apiHostName"></param>
        /// <param name="apiPort"></param>
        /// <returns></returns>
        public static string CreateApiBasePath(bool useSsl, string apiHostName, int apiPort)
        {
            var sb = new StringBuilder();

            if (useSsl)
            { sb.Append("https://"); }
            else
            { sb.Append("http://"); }

            sb.Append(apiHostName);
            sb.Append(':');
            sb.Append(apiPort);
            sb.Append("/api");

            return sb.ToString();
        }
    }
}
