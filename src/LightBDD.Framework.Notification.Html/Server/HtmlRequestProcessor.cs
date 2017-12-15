using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LightBDD.Framework.Notification.Html.Server
{
    internal class HtmlRequestProcessor : IHttpRequestProcessor
    {
        public bool Match(HttpListenerRequest request)
        {
            return request.Is(HttpMethod.Get, "/progress.html");
        }

        public async Task ProcessRequestAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/HTML";
            response.ContentEncoding = Encoding.UTF8;

            using (var input = GetHtmlResource())
                await input.CopyToAsync(response.OutputStream);
        }

        private static Stream GetHtmlResource()
        {
            return typeof(HtmlRequestProcessor).GetTypeInfo().Assembly.GetManifestResourceStream("LightBDD.Framework.Notification.Html.progress.html");
        }
    }
}