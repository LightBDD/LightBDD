using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using LightBDD.Core.Execution.Implementation;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Exception indicating that step or scenario thrown an exception.
    /// It's purpose is to allow LightBDD engine to process exception and eventually report them back to the underlying test frameworks without exposing LightBDD internal stack frames.
    /// 
    /// The inner exception represents original one that has been thrown by step/scenario.
    /// </summary>
    [DebuggerStepThrough]
    public class ScenarioExecutionException : Exception
    {
        /// <summary>
        /// Constructor allowing to specify inner exception
        /// </summary>
        /// <param name="inner">Inner exception.</param>
        public ScenarioExecutionException(Exception inner) : base(string.Empty, inner ?? throw new ArgumentNullException(nameof(inner))) { }

        /// <summary>
        /// Returns <see cref="ExceptionDispatchInfo"/> of the original exception, allowing to rethrow it.
        /// </summary>
        public ExceptionDispatchInfo GetOriginal()
        {
            return ExceptionDispatchInfo.Capture(InnerException);
        }

        internal static bool TryWrap(Exception exception, out Exception result)
        {
            if (exception is ScenarioExecutionException || exception is StepExecutionException)
            {
                result = exception;
                return false;
            }
            result = new ScenarioExecutionException(exception);
            return true;
        }

        internal static Exception WrapIfNeeded(Exception exception) =>
            TryWrap(exception, out var result) ? result : exception;
    }
}