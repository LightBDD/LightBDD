using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class AsyncStepSynchronizationContext : SynchronizationContext
    {
        private readonly SynchronizationContext _inner;
        private int _counter = 1;
        private readonly TaskCompletionSource<bool> _resetEvent = new TaskCompletionSource<bool>();
        private readonly ConcurrentQueue<Exception> _exceptions = new ConcurrentQueue<Exception>();

        private AsyncStepSynchronizationContext(SynchronizationContext inner)
        {
            _inner = inner;
        }

        public static AsyncStepSynchronizationContext InstallNew()
        {
            var ctx = new AsyncStepSynchronizationContext(Current ?? new SynchronizationContext());
            SetSynchronizationContext(ctx);
            return ctx;
        }

        public void RestoreOriginal()
        {
            if (Current == this)
                SetSynchronizationContext(_inner);
        }

        public Task WaitForTasksAsync()
        {
            OperationCompleted();
            return _resetEvent.Task;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            OperationStarted();
            _inner.Post(s => RunWithSelf(d, s), state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            OperationStarted();
            _inner.Send(s => RunWithSelf(d, s), state);
        }

        private void RunWithSelf(SendOrPostCallback d, object s)
        {
            var previous = Current;
            SetSynchronizationContext(this);
            try
            {
                d(s);
            }
            catch (Exception e)
            {
                _exceptions.Enqueue(e);
            }
            finally
            {
                SetSynchronizationContext(previous);
                OperationCompleted();
            }
        }

        public override void OperationStarted()
        {
            Interlocked.Increment(ref _counter);
        }

        public override void OperationCompleted()
        {
            if (Interlocked.Decrement(ref _counter) != 0)
                return;

            var exceptions = _exceptions.ToArray();
            if (!exceptions.Any())
                _resetEvent.TrySetResult(true); //TODO: why Try is needed?
            else if (exceptions.Length == 1)
                _resetEvent.TrySetException(exceptions);
            else
                _resetEvent.TrySetException(new AggregateException(exceptions));
        }
    }
}