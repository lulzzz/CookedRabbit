using System;
using System.Net.Http;

namespace CookedRabbit.Library.Utilities
{
    public static class ApiHelpers
    {
        public static HttpRequestMessage CreateRequest(string apiPath, string subPath, HttpMethod httpMethod)
        {
            var apiUri = new Uri($"{apiPath}/{subPath}");
            return new HttpRequestMessage(httpMethod, apiUri);
        }

        public static HttpRequestMessage CreateRequest(string hostName, int portNumber, string path, HttpMethod httpMethod)
        {
            var uri = new Uri($"{hostName}:{portNumber}/api/{path}");
            return new HttpRequestMessage(httpMethod, uri);
        }

        public static DateTime UnixSecondsToDateTime(long unixSeconds, bool returnLocalTime = false)
        {
            DateTime dateTime;

            if (returnLocalTime) { dateTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).LocalDateTime; }
            else { dateTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).DateTime; }

            return dateTime;
        }
    }
}
