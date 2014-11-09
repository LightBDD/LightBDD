using System;
using System.Diagnostics;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD.Execution
{
    [DebuggerStepThrough]
    internal class Step : IStep
    {
        private readonly Action _action;
        private readonly Func<Type, ResultStatus> _mapping;
        private readonly StepResult _result;
        public IStepResult GetResult() { return _result; }

        public Step(Action action, string stepName, int stepNumber, Func<Type, ResultStatus> mapping)
        {
            _action = action;
            _mapping = mapping;
            _result = new StepResult(stepNumber, new StepName(stepName), ResultStatus.NotRun);
        }

        public void Invoke(IProgressNotifier progressNotifier, int totalCount)
        {
            try
            {
                progressNotifier.NotifyStepStart(_result.Name, _result.Number, totalCount);
                _result.SetExecutionStart(DateTimeOffset.UtcNow);
                MeasuredInvoke();
                _result.SetStatus(ResultStatus.Passed);
            }
            catch (Exception e)
            {
                _result.SetStatus(_mapping(e.GetType()), e.Message);
                throw;
            }
            finally
            {
                progressNotifier.NotifyStepFinished(_result, totalCount);
            }
        }

        private void MeasuredInvoke()
        {
            var watch = new Stopwatch();
            try
            {
                _result.SetExecutionStart(DateTimeOffset.UtcNow);
                watch.Start();
                _action();
            }
            finally
            {
                _result.SetExecutionTime(watch.Elapsed);
            }
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}