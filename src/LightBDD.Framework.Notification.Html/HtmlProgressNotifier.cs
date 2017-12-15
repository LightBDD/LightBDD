using System;
using System.IO;
using LightBDD.Framework.Notification.Html.Messaging;
using LightBDD.Framework.Notification.Html.Server;

namespace LightBDD.Framework.Notification.Html
{
    public class HtmlProgressNotifier : IncrementalJsonProgressNotifier
    {
        private readonly HttpServer _server;
        private readonly MessageStream _messageStream;

        public HtmlProgressNotifier(Uri baseAddress)
        {
            _messageStream = new MessageStream();
            _server = HttpServer.Start(baseAddress, new HtmlRequestProcessor(), new ProgressRequestProcessor(_messageStream));
        }

        protected override void OnFinish()
        {
            _messageStream.Finish();
            _server.Dispose();
        }

        protected override void WriteMessage(Action<StreamWriter> writeFn)
        {
            _messageStream.WriteMessage(writeFn);
        }
    }
}