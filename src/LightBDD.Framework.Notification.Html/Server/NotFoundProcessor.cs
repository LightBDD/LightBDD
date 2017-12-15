using System.Net;
using System.Threading.Tasks;

namespace LightBDD.Framework.Notification.Html.Server
{
    internal class NotFoundProcessor : IHttpRequestProcessor
    {
        public bool Match(HttpListenerRequest request)
        {
            return true;
        }

        public Task ProcessRequestAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.StatusCode = 404;
            return Task.FromResult(0);
        }
    }
}