using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Implementation;

namespace LightBDD.Core.Execution
{
    [DebuggerStepThrough]
    public class ScenarioExecutionFlow
    {
        public static ScenarioExceptionWrappingAwaitable WrapScenarioExceptions(Task targetTask)
        {
            return new ScenarioExceptionWrappingAwaitable(targetTask);
        }

        public static ScenarioExceptionWrappingAwaitable<T> WrapScenarioExceptions<T>(Task<T> targetTask)
        {
            return new ScenarioExceptionWrappingAwaitable<T>(targetTask);
        }

        [DebuggerStepThrough]
        public struct ScenarioExceptionWrappingAwaitable : ICriticalNotifyCompletion
        {
            private readonly Task _task;
            private TaskAwaiter _awaiter;

            internal ScenarioExceptionWrappingAwaitable(Task task)
            {
                _task = task;
                _awaiter = _task.GetAwaiter();
            }

            public ScenarioExceptionWrappingAwaitable GetAwaiter() => this;

            public bool IsCompleted => _awaiter.IsCompleted;
            [SecuritySafeCritical]
            public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

            public void GetResult()
            {
                if (!_task.IsCompleted)
                    _task.Wait();

                if (_task.Exception != null)
                {
                    var exception = _task.Exception.InnerExceptions[0];
                    if (exception is ScenarioExecutionException || exception is StepExecutionException)
                        ExceptionDispatchInfo.Capture(exception).Throw();
                    throw new ScenarioExecutionException(exception);
                }
                _awaiter.GetResult();
            }
        }

        [DebuggerStepThrough]
        public struct ScenarioExceptionWrappingAwaitable<T> : ICriticalNotifyCompletion
        {
            private readonly Task<T> _task;
            private TaskAwaiter<T> _awaiter;

            internal ScenarioExceptionWrappingAwaitable(Task<T> task)
            {
                _task = task;
                _awaiter = _task.GetAwaiter();
            }

            public ScenarioExceptionWrappingAwaitable<T> GetAwaiter() => this;

            public bool IsCompleted => _awaiter.IsCompleted;
            [SecuritySafeCritical]
            public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

            public T GetResult()
            {
                if (!_task.IsCompleted)
                    _task.Wait();

                if (_task.Exception != null)
                {
                    var exception = _task.Exception.InnerExceptions[0];
                    if (exception is ScenarioExecutionException || exception is StepExecutionException)
                        ExceptionDispatchInfo.Capture(exception).Throw();
                    throw new ScenarioExecutionException(exception);
                }

                return _awaiter.GetResult();

            }
        }
    }
}