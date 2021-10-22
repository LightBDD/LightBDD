using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Formatting;

namespace LightBDD.Framework.Messaging
{
    public class MessageListener : IDisposable
    {
        private readonly IMessageSource _source;
        private readonly ConcurrentStack<object> _messages = new ConcurrentStack<object>();
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
        private event Action<object> OnMessage;

        private MessageListener(IMessageSource source)
        {
            _source = source;
            _source.OnMessage += OnHandle;
        }

        private void OnHandle(object msg)
        {
            _messages.Push(msg);
            OnMessage?.Invoke(msg);
        }

        public static MessageListener Start(IMessageSource source) => new MessageListener(source);

        public IEnumerable<T> GetMessages<T>() => _messages.OfType<T>();

        public void Dispose() => _source.OnMessage -= OnHandle;

        public Task<T> EnsureReceived<T>(Expression<Func<T, bool>> predicate, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
            => EnsureReceived(predicate.Compile(), $"No message received matching criteria: {predicate}", timeout, cancellationToken);

        public async Task<T> EnsureReceived<T>(Func<T, bool> predicate, string errorMessage, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            bool PredicateFn(T m) => IsValidMessage(m, predicate);

            using var waiter = new MessageWaiter<T>(this, PredicateFn, errorMessage);
            var msg = _messages.OfType<T>().FirstOrDefault(PredicateFn);
            return msg != null ? msg : await waiter.WaitAsync(timeout ?? DefaultTimeout, CancellationToken.None);
        }

        private static bool IsValidMessage<T>(T msg, Func<T, bool> predicate)
        {
            try
            {
                return predicate.Invoke(msg);
            }
            catch (Exception ex)
            {
                throw new MessagePredicateEvaluationException(ex, msg);
            }
        }

        class MessageWaiter<T> : IDisposable
        {
            private readonly MessageListener _listener;
            private readonly Func<T, bool> _predicate;
            private readonly string _errorMessage;
            private readonly TaskCompletionSource<T> _tcs = new TaskCompletionSource<T>();

            public MessageWaiter(MessageListener listener, Func<T, bool> predicate, string errorMessage)
            {
                _listener = listener;
                _predicate = predicate;
                _errorMessage = errorMessage;
                _listener.OnMessage += OnMessage;
            }

            public void Dispose()
            {
                _tcs.TrySetCanceled();
                StopListening();
            }

            void OnMessage(object msg)
            {
                try
                {
                    if (msg is T message && _predicate(message) && _tcs.TrySetResult(message))
                        StopListening();
                }
                catch (Exception ex)
                {
                    if (_tcs.TrySetException(ex))
                        StopListening();
                }
            }

            private void StopListening() => _listener.OnMessage -= OnMessage;

            public async Task<T> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
            {
                if (await Task.WhenAny(_tcs.Task, Task.Delay(timeout, cancellationToken)) == _tcs.Task)
                    return await _tcs.Task;
                throw new TimeoutException($"Failed to receive {typeof(T).Name} within {timeout.FormatPretty()}: {_errorMessage}");
            }
        }
    }
}