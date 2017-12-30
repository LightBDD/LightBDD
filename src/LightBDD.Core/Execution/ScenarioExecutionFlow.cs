using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Implementation;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Class helping to wrap scenario/step specific exceptions with <see cref="ScenarioExecutionException"/> that allows to propagate original exception with bypassing LightBDD internal stack frames.
    /// </summary>
    [DebuggerStepThrough]
    public class ScenarioExecutionFlow
    {
        /// <summary>
        /// Wraps <paramref name="targetTask"/> with awaitable that will wrap task exception with <see cref="ScenarioExecutionException"/> without recording exception capture on exception's stack trace
        /// </summary>
        public static ScenarioExceptionWrappingAwaitable WrapScenarioExceptions(Task targetTask)
        {
            return new ScenarioExceptionWrappingAwaitable(targetTask);
        }

        /// <summary>
        /// Wraps <paramref name="targetTask"/> with awaitable that will wrap task exception with <see cref="ScenarioExecutionException"/> without recording exception capture on exception's stack trace
        /// </summary>
        public static ScenarioExceptionWrappingAwaitable<T> WrapScenarioExceptions<T>(Task<T> targetTask)
        {
            return new ScenarioExceptionWrappingAwaitable<T>(targetTask);
        }

        /// <summary>
        /// Awaitable allowing to wrap task exception with <see cref="ScenarioExecutionException"/> without recording exception capture on exception's stack trace
        /// </summary>
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
            /// <summary>
            /// Returns self.
            /// </summary>
            public ScenarioExceptionWrappingAwaitable GetAwaiter() => this;
            /// <summary>
            /// Returns true if task is completed.
            /// </summary>
            public bool IsCompleted => _awaiter.IsCompleted;

            /// <inheritdoc />
            [SecuritySafeCritical]
            public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

            /// <inheritdoc />
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);
            /// <summary>
            /// Returns waits for task to finish and throws <see cref="ScenarioExecutionException"/> if task failed.
            /// </summary>
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

        /// <summary>
        /// Awaitable allowing to wrap task exception with <see cref="ScenarioExecutionException"/> without recording exception capture on exception's stack trace
        /// </summary>
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

            /// <summary>
            /// Returns self.
            /// </summary>
            public ScenarioExceptionWrappingAwaitable<T> GetAwaiter() => this;
            /// <summary>
            /// Returns true if task is completed.
            /// </summary>
            public bool IsCompleted => _awaiter.IsCompleted;
            /// <inheritdoc />
            [SecuritySafeCritical]
            public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);
            /// <inheritdoc />
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);
            /// <summary>
            /// Waits for task to finish and returns task result or throws <see cref="ScenarioExecutionException"/> if task failed.
            /// </summary>
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