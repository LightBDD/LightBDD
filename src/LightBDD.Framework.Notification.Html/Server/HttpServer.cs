using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LightBDD.Framework.Notification.Html.Server
{
    internal class HttpServer : IDisposable
    {
        private static readonly IHttpRequestProcessor NotImplementedProcessor = new NotFoundProcessor();

        public Uri BaseAddress { get; }
        private readonly HttpListener _listener;
        private readonly Task _listenerTask;
        private readonly IHttpRequestProcessor[] _processors;
        private readonly IDictionary<Guid, Task> _pendingTasks = new ConcurrentDictionary<Guid, Task>();

        public static HttpServer Start(Uri baseAddress, params IHttpRequestProcessor[] processors)
        {
            return new HttpServer(baseAddress, processors);
        }

        private HttpServer(Uri baseAddress, IHttpRequestProcessor[] processors)
        {
            _processors = processors;
            BaseAddress = baseAddress;

            _listener = new HttpListener();
            _listener.Prefixes.Add(BaseAddress.ToString());
            _listener.Start();
            _listenerTask = Task.Run(Listen);
        }

        private async Task Listen()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var ctx = await _listener.GetContextAsync();
                    ScheduleProcessRequest(ctx);
                }
                catch (Exception) { }
            }
            await Task.WhenAll(_pendingTasks.Values);
        }

        private void ScheduleProcessRequest(HttpListenerContext ctx)
        {
            var id = Guid.NewGuid();
            var task = new Task(async () =>
            {
                await ProcessRequest(ctx);
                _pendingTasks.Remove(id);
            });
            _pendingTasks.Add(id, task);
            task.Start();
        }

        private async Task ProcessRequest(HttpListenerContext ctx)
        {
            try
            {
                var processor = _processors.FirstOrDefault(p => p.Match(ctx.Request)) ?? NotImplementedProcessor;
                await processor.ProcessRequestAsync(ctx.Request, ctx.Response);
            }
            catch (Exception e)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ctx.Response.ContentType = "text";
                var buffer = Encoding.UTF8.GetBytes(e.ToString());
                ctx.Response.ContentEncoding = Encoding.UTF8;
                ctx.Response.ContentLength64 = buffer.Length;
                ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                ctx.Response.Close();
            }
        }

        public void Dispose()
        {
            if (!_listener.IsListening)
                return;
            _listener.Stop();
            _listenerTask.Wait();
        }
    }
}