using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Scheduling.Implementation;

internal class DedicatedThreadScenarioExecutionScheduler : IScenarioExecutionScheduler, IDisposable
{
    private readonly ConcurrentQueue<SingleThreadScenarioRunner> _all = new();
    private readonly ConcurrentStack<SingleThreadScenarioRunner> _free = new();
    private readonly CancellationTokenSource _cts = new();

    public Task<T> Schedule<T>(Func<Task<T>> scenarioFn)
    {
        if (!_free.TryPop(out var runner))
            runner = SpawnRunner();
        return runner.Run(scenarioFn);
    }

    private SingleThreadScenarioRunner SpawnRunner()
    {
        var runner = new SingleThreadScenarioRunner(_cts.Token);
        runner.OnFinish += _free.Push;
        _all.Enqueue(runner);
        return runner;
    }

    public void Dispose()
    {
        _cts.Cancel();
        foreach (var runner in _all)
            runner.Dispose();
    }

    class SingleThreadScenarioRunner : SynchronizationContext, IDisposable
    {
        private readonly CancellationToken _cancellationToken;
        private readonly Thread _thread;
        private readonly BlockingCollection<Action> _actions = new();
        private readonly SemaphoreSlim _completed = new(0);
        public event Action<SingleThreadScenarioRunner> OnFinish;
        public SingleThreadScenarioRunner(CancellationToken token)
        {
            _cancellationToken = token;
            _thread = new Thread(ThreadLoop) { IsBackground = true, Name = "SingleThreadScenarioRunner" };
            _thread.Start();
        }

        private void ThreadLoop()
        {
            SetSynchronizationContext(this);
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _actions.Take(_cancellationToken).Invoke();
                }
                catch
                {
                    /* ignore */
                }

                if (_actions.Count == 0)
                    _completed.Release();
            }
        }

        public override void Post(SendOrPostCallback d, object state) => _actions.Add(() => d(state), _cancellationToken);

        public Task<T> Run<T>(Func<Task<T>> scenarioFn)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();

            async void RunAsAction()
            {
                T result = default;
                Exception exception = null;
                try
                {
                    result = await scenarioFn();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                try
                {
                    await _completed.WaitAsync(_cancellationToken);
                }
                catch
                {
                    // ignored
                }

                OnFinish?.Invoke(this);

                if (exception != null)
                    taskCompletionSource.TrySetException(exception);
                else
                    taskCompletionSource.TrySetResult(result);
            }

            _actions.Add(RunAsAction, _cancellationToken);
            return taskCompletionSource.Task;
        }

        public void Dispose() => _thread.Join();
    }
}