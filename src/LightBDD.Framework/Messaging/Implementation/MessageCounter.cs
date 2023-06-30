using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Diagnostics;

namespace LightBDD.Framework.Messaging.Implementation
{
    internal class MessageCounter<TMessage> : IDisposable
    {
        private readonly MessageListener _listener;
        private readonly int _expectedCount;
        private readonly Func<TMessage, bool> _predicate;
        private volatile int _current = 0;
        private readonly HashSet<TMessage> _messageHash = new();
        private readonly TaskCompletionSource<bool> _tcs = new();

        private bool IsFinished => _tcs.Task.IsCompleted;

        public MessageCounter(MessageListener listener, int expectedCount, Func<TMessage, bool> predicate)
        {
            _listener = listener;
            _expectedCount = expectedCount;
            _predicate = predicate;
            _listener.OnMessage += HandleMessage;
        }

        private void HandleMessage(object msg)
        {
            try
            {
                if (msg is TMessage message
                    && _predicate.Invoke(message)
                    && _messageHash.Add(message)
                    && Interlocked.Increment(ref _current) >= _expectedCount)
                    _tcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                _tcs.TrySetException(ex);
            }
        }

        public void Dispose()
        {
            StopListening();
            _tcs.TrySetCanceled();
        }

        private void StopListening()
        {
            _listener.OnMessage -= HandleMessage;
        }

        public async Task<TMessage[]> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            foreach (var message in _listener.GetMessages<TMessage>())
            {
                HandleMessage(message);
                if (IsFinished)
                    break;
            }

            await Task.WhenAny(_tcs.Task, Task.Delay(timeout, cancellationToken));
            StopListening();
            cancellationToken.ThrowIfCancellationRequested();

            var messages = _listener.GetMessages<TMessage>().Where(_predicate).Take(_expectedCount).ToArray();
            if (messages.Length >= _expectedCount)
                return messages;

            throw new TimeoutException($"Failed to receive matching {_expectedCount} {typeof(TMessage).Name} message(s) within {timeout.FormatPretty()}:\n\nReceived {messages.Length} messages matching criteria:\n{ObjectFormatter.DumpMany(messages)}\n\nLast recorded {typeof(TMessage).Name} messages:\n{ObjectFormatter.DumpMany(_listener.GetMessages<TMessage>().Take(10))}");
        }
    }
}