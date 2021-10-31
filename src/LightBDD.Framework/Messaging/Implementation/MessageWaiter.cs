using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Diagnostics;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Messaging.Implementation
{
    internal class MessageWaiter<TMessage> : IDisposable
    {
        private readonly MessageListener _listener;
        private readonly Func<TMessage, bool> _predicate;
        private readonly TaskCompletionSource<TMessage> _tcs = new TaskCompletionSource<TMessage>();

        public MessageWaiter(MessageListener listener, Func<TMessage, bool> predicate)
        {
            _listener = listener;
            _predicate = predicate;
            _listener.OnMessage += HandleMessage;
        }

        public void Dispose()
        {
            _tcs.TrySetCanceled();
            StopListening();
        }

        private void HandleMessage(object msg)
        {
            try
            {
                if (msg is TMessage message && _predicate(message) && _tcs.TrySetResult(message))
                    StopListening();
            }
            catch (Exception ex)
            {
                if (_tcs.TrySetException(ex))
                    StopListening();
            }
        }

        private void StopListening() => _listener.OnMessage -= HandleMessage;

        public async Task<TMessage> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            foreach (var message in _listener.GetMessages<TMessage>())
            {
                HandleMessage(message);
                if (_tcs.Task.IsCompleted)
                    return await _tcs.Task;
            }

            if (await Task.WhenAny(_tcs.Task, Task.Delay(timeout, cancellationToken)) == _tcs.Task)
                return await _tcs.Task;
            cancellationToken.ThrowIfCancellationRequested();
            throw new TimeoutException($"Failed to receive matching {typeof(TMessage).Name} within {timeout.FormatPretty()}.\n\nLast recorded {typeof(TMessage).Name} messages:\n{ObjectFormatter.DumpMany(_listener.GetMessages<TMessage>().Take(10))}");
        }
    }
}