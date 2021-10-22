using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Messaging
{
    public class MessageListener : IDisposable
    {
        private readonly IMessageSource _source;
        private readonly ConcurrentStack<object> _queue = new ConcurrentStack<object>();
        private MessageListener(IMessageSource source)
        {
            _source = source;
            _source.OnMessage += OnHandle;
        }

        private void OnHandle(object msg) => _queue.Push(msg);

        public static MessageListener Start(IMessageSource source) => new MessageListener(source);

        public IEnumerable<T> GetMessages<T>() => _queue.OfType<T>();

        public void Dispose() => _source.OnMessage -= OnHandle;
    }
}