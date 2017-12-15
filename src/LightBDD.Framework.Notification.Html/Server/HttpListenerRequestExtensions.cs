using System;
using System.Net;
using System.Net.Http;

namespace LightBDD.Framework.Notification.Html.Server
{
    internal static class HttpListenerRequestExtensions
    {
        public static bool Is(this HttpListenerRequest request, HttpMethod method, string route)
        {
            return string.Equals(request.HttpMethod, method.Method, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(request.RawUrl, route, StringComparison.OrdinalIgnoreCase);
        }
    }
}