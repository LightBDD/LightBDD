using System.Net;
using System.Threading.Tasks;

namespace LightBDD.Framework.Notification.Html.Server
{
    internal interface IHttpRequestProcessor
    {
        bool Match(HttpListenerRequest request);
        Task ProcessRequestAsync(HttpListenerRequest request, HttpListenerResponse response);
    }
}