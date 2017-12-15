using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Notification.Html.Messaging;

namespace LightBDD.Framework.Notification.Html.Server
{
    internal class ProgressRequestProcessor : IHttpRequestProcessor
    {
        private readonly MessageStream _messageStream;

        public ProgressRequestProcessor(MessageStream messageStream)
        {
            _messageStream = messageStream;
        }

        public bool Match(HttpListenerRequest request)
        {
            return request.Is(HttpMethod.Get, "/progress.jsonl");
        }

        public async Task ProcessRequestAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "text/plain";
            response.SendChunked = true;

            var node = await _messageStream.GetFirstEventAsync();
            while (node != null)
            {
                await response.OutputStream.WriteAsync(node.Data, 0, node.Data.Length);
                await response.OutputStream.FlushAsync();
                node = await node.GetNextAsync();
            }
        }
    }
}