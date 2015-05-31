using System;
using System.Diagnostics;
using LightBDD.Execution.Exceptions;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class Step : IStep
    {
        private readonly Action _action;
        private readonly Func<Type, ResultStatus> _mapping;
        private readonly StepResult _result;
        public IStepResult GetResult() { return _result; }

        public Step(Action action, string stepTypeName, string stepName, int stepNumber, Func<Type, ResultStatus> mapping)
        {
            _action = action;
            _mapping = mapping;
            _result = new StepResult(stepNumber, new StepName(stepName, stepTypeName), ResultStatus.NotRun);
        }

        public void Invoke(ExecutionContext context)
        {
            try
            {
                context.CurrentStep = this;
                context.ProgressNotifier.NotifyStepStart(_result.Name, _result.Number, context.TotalStepCount);
                _result.SetExecutionStart(DateTimeOffset.UtcNow);
                MeasuredInvoke();
                _result.SetStatus(ResultStatus.Passed);
            }
            catch (StepBypassException e)
            {
                _result.SetStatus(ResultStatus.Bypassed, e.Message);
            }
            catch (Exception e)
            {
                _result.SetStatus(_mapping(e.GetType()), e.Message);
                throw;
            }
            finally
            {
                context.CurrentStep = null;
                context.ProgressNotifier.NotifyStepFinished(_result, context.TotalStepCount);
            }
        }

        public void Comment(ExecutionContext context, string comment)
        {
            _result.AddComment(comment);
            context.ProgressNotifier.NotifyStepComment(_result.Number, context.TotalStepCount, comment);
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