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
    /// <summary>
    /// MessageListener class allowing to listen for and record messages for further assertions.
    /// </summary>
    public class MessageListener : IDisposable
    {
        private readonly IMessageSource _source;
        private readonly ConcurrentStack<object> _messages = new ConcurrentStack<object>();
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
        private readonly CancellationTokenSource _listenerDisposedTokenSource = new CancellationTokenSource();
        internal event Action<object> OnMessage;

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

        /// <summary>
        /// Starts listening for messages coming from specified <paramref name="source"/>.
        ///<br/>
        /// Please note, the listener should be disposed after usage, in order to stop listening.
        /// </summary>
        /// <param name="source">Message source.</param>
        public static MessageListener Start(IMessageSource source) => new MessageListener(source);

        /// <summary>
        /// Returns all received messages of specified type, in order from latest to oldest.
        /// </summary>
        /// <typeparam name="TMessage">Message type.</typeparam>
        /// <returns>Collection of received messages.</returns>
        public IEnumerable<TMessage> GetMessages<TMessage>() => _messages.OfType<TMessage>();

        /// <summary>
        /// Disposes listener making it no longer listening for new messages. <br/>
        /// Upon dispose, the received messages are cleared and all pending receive method calls aborted.
        /// </summary>
        public void Dispose()
        {
            _source.OnMessage -= OnHandle;
            _messages.Clear();
            _listenerDisposedTokenSource.Cancel();
        }

        /// <summary>
        /// Ensures the message of type <typeparamref name="TMessage"/> and matching predicate <paramref name="predicate"/> is received.<br/>
        /// If one or more matching messages were already received by listener, the latest matching message is returned immediately.<br/>
        /// If no matching messages were received yet, the method listens for upcoming messages and returns when matching one arrives, timeout occurs or <paramref name="cancellationToken"/> is cancelled.<br/>
        /// </summary>
        /// <typeparam name="TMessage">Type of message to receive.</typeparam>
        /// <param name="predicate">Predicate that message have to match to be returned.</param>
        /// <param name="timeout">Timeout for how long the method should await for matching message to arrive. If <c>null</c>, a default value of 10s will be used.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Latest matching message</returns>
        /// <exception cref="TimeoutException">Thrown when timeout occurs and no matching message was received.</exception>
        /// <exception cref="MessagePredicateEvaluationException">Thrown when provided predicate failed evaluation on received message.</exception>
        public Task<TMessage> EnsureReceived<TMessage>(Expression<Func<TMessage, bool>> predicate, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
            => EnsureReceived(predicate.Compile(), $"No message received matching criteria: {predicate}", timeout, cancellationToken);

        /// <summary>
        /// Ensures the message of type <typeparamref name="TMessage"/> and matching predicate <paramref name="predicate"/> is received.<br/>
        /// If one or more matching messages were already received by listener, the latest matching message is returned immediately.<br/>
        /// If no matching messages were received yet, the method listens for upcoming messages and returns when matching one arrives, timeout occurs or <paramref name="cancellationToken"/> is cancelled.<br/>
        /// </summary>
        /// <typeparam name="TMessage">Type of message to receive.</typeparam>
        /// <param name="predicate">Predicate that message have to match to be returned.</param>
        /// <param name="errorMessage">An error message to include in <seealso cref="TimeoutException"/> when timeout occurs.</param>
        /// <param name="timeout">Timeout for how long the method should await for matching message to arrive. If <c>null</c>, a default value of 10s will be used.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Latest matching message</returns>
        /// <exception cref="TimeoutException">Thrown when timeout occurs and no matching message was received.</exception>
        /// <exception cref="MessagePredicateEvaluationException">Thrown when provided predicate failed evaluation on received message.</exception>
        public async Task<TMessage> EnsureReceived<TMessage>(Func<TMessage, bool> predicate, string errorMessage, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            bool PredicateFn(TMessage m) => IsValidMessage(m, predicate);

            using var waiter = new MessageWaiter<TMessage>(this, PredicateFn, errorMessage);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _listenerDisposedTokenSource.Token);
            waiter.ProcessAlreadyCaptured(_messages);
            return await waiter.WaitAsync(timeout ?? DefaultTimeout, cts.Token);
        }

        /// <summary>
        /// Ensures the <paramref name="count"/> number of messages of type <typeparamref name="TMessage"/> and matching predicate <paramref name="predicate"/> are received.<br/>
        /// If all required messages are already received by listener, the method returns immediately.<br/>
        /// If none or not all messages are received yet, the method listens for upcoming messages and returns when all are received, timeout occurs or <paramref name="cancellationToken"/> is cancelled.<br/>
        /// If more than <paramref name="count"/> messages matches the criteria, the latest <paramref name="count"/> messages will be returned.
        /// </summary>
        /// <typeparam name="TMessage">Type of message to receive.</typeparam>
        /// <param name="count">Number of messages that should be received</param>
        /// <param name="predicate">Predicate that message have to match to be returned.</param>
        /// <param name="errorMessageFn">Function accepting list of received messages and returning an error message to include in <seealso cref="TimeoutException"/> when timeout occurs.</param>
        /// <param name="timeout">Timeout for how long the method should await for matching messages to arrive. If <c>null</c>, a default value of 10s will be used.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of <paramref name="count"/> requested messages, ordered from latest to oldest.</returns>
        public async Task<IReadOnlyList<TMessage>> EnsureReceivedMany<TMessage>(int count, Func<TMessage, bool> predicate, Func<IReadOnlyList<TMessage>, string> errorMessageFn, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            bool PredicateFn(TMessage m) => IsValidMessage(m, predicate);
            using var counter = new MessageCounter<TMessage>(this, count, PredicateFn, errorMessageFn);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _listenerDisposedTokenSource.Token);
            return await counter.WaitAsync(timeout ?? DefaultTimeout, cts.Token);
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

        private class MessageWaiter<T> : IDisposable
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

            public void ProcessAlreadyCaptured(IEnumerable<object> messages)
            {
                foreach (var msg in messages)
                {
                    if (_tcs.Task.IsCompleted)
                        return;
                    OnMessage(msg);
                }
            }

            public void Dispose()
            {
                _tcs.TrySetCanceled();
                StopListening();
            }

            private void OnMessage(object msg)
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
                cancellationToken.ThrowIfCancellationRequested();
                throw new TimeoutException($"Failed to receive {typeof(T).Name} within {timeout.FormatPretty()}: {_errorMessage}");
            }
        }

        private class MessageCounter<TMessage> : IDisposable
        {
            private readonly MessageListener _listener;
            private readonly int _expectedCount;
            private readonly Func<TMessage, bool> _predicate;
            private readonly Func<IReadOnlyList<TMessage>, string> _errorMessageFn;
            private volatile int _current = 0;
            private readonly HashSet<TMessage> _messageHash = new HashSet<TMessage>();
            private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

            private bool IsFinished => _tcs.Task.IsCompleted;

            public MessageCounter(MessageListener listener, int expectedCount, Func<TMessage, bool> predicate, Func<IReadOnlyList<TMessage>,string> errorMessageFn)
            {
                _listener = listener;
                _expectedCount = expectedCount;
                _predicate = predicate;
                _errorMessageFn = errorMessageFn;
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

                throw new TimeoutException($"Received {messages.Length} out of {_expectedCount} {typeof(TMessage).Name} message(s) within {timeout.FormatPretty()}: {_errorMessageFn(messages)}");
            }
        }
    }
}