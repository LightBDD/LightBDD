using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.XUnit.UnitTests.Helpers
{
    class MessageSink : LongLivedMarshalByRefObject, IMessageSink
    {
        private readonly ManualResetEventSlim _finished = new ManualResetEventSlim(false);
        private readonly ConcurrentQueue<IMessageSinkMessage> _messages = new ConcurrentQueue<IMessageSinkMessage>();
        public bool OnMessage(IMessageSinkMessage message)
        {
            _messages.Enqueue(message);
            if (message is ITestAssemblyFinished)
                _finished.Set();
            return true;
        }

        public T[] GetMessages<T>() { return _messages.OfType<T>().ToArray(); }

        public void WaitTillFinished()
        {
            _finished.Wait(TimeSpan.FromSeconds(1));
        }
    }
}