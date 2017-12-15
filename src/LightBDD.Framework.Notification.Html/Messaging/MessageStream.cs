using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LightBDD.Framework.Notification.Html.Messaging
{
    internal class MessageStream
    {
        private readonly MessageNode _root = new MessageNode(null);
        private MessageNode _last;
        private readonly MemoryStream _buffer = new MemoryStream();
        private readonly StreamWriter _writer;

        public MessageStream()
        {
            _last = _root;
            _writer = new StreamWriter(_buffer, Encoding.UTF8);
        }

        public void WriteMessage(Action<StreamWriter> writeFn)
        {
            lock (_buffer)
            {
                _buffer.Seek(0, SeekOrigin.Begin);
                writeFn(_writer);
                _writer.Write('\n');
                _writer.Flush();
                _buffer.SetLength(_buffer.Position);
                _last = _last.Append(new MessageNode(_buffer.ToArray()));
            }
        }

        public void Finish()
        {
            _last = _last.Append(null);
            _writer.Dispose();
        }

        public Task<MessageNode> GetFirstEventAsync()
        {
            return _root.GetNextAsync();
        }
    }
}
