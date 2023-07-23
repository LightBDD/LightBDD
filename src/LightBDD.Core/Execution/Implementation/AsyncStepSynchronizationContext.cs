using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Implementation
{
    //TODO: review after migration
    internal class AsyncStepSynchronizationContext : SynchronizationContext
    {
        private readonly SynchronizationContext _inner;
        private int _counter = 1;
        private readonly TaskCompletionSource<bool> _resetEvent = new();
        private readonly ConcurrentQueue<Exception> _exceptions = new();

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

        public Task CompleteAsync()
        {
            RestoreOriginal();
            return WaitForTasksAsync();
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

            _inner.Post(new RunWithSelfDelegate(d, this).Run, state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            OperationStarted();

            _inner.Send(new RunWithSelfDelegate(d, this).Run, state);
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
                _resetEvent.TrySetException(WrapIfNeeded(exceptions));
            else
                _resetEvent.TrySetException(new AggregateException(exceptions));
        }

        private IEnumerable<Exception> WrapIfNeeded(Exception[] exceptions)
        {
            return exceptions.Select(ScenarioExecutionException.WrapIfNeeded);
        }
        private struct RunWithSelfDelegate
        {
            private readonly SendOrPostCallback _callback;
            private readonly AsyncStepSynchronizationContext _context;

            public RunWithSelfDelegate(SendOrPostCallback callback, AsyncStepSynchronizationContext context)
            {
                _callback = callback;
                _context = context;
            }

            public void Run(object state)
            {
                _context.RunWithSelf(_callback, state);
            }
        }

        public static async Task Execute(Func<Task> action)
        {
            var ctx = InstallNew();
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                ctx._exceptions.Enqueue(ex);
            }

            await ctx.CompleteAsync();
        }
    }
}