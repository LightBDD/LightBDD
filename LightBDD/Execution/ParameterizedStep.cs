using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using LightBDD.Execution.Parameters;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD.Execution
{
    internal class ParameterizedStep<TContext> : IStep
    {
        private readonly TContext _context;
        private readonly Action<StepType, TContext, object[]> _action;
        private readonly IStepParameter<TContext>[] _parameters;
        private readonly string _stepNameFormat;
        private readonly int _stepNumber;
        private readonly Func<Type, ResultStatus> _mapping;
        private StepResult _result;
        public IStepResult GetResult() { return _result; }

        [DebuggerStepThrough]
        public ParameterizedStep(TContext context, Action<StepType, TContext, object[]> action, IStepParameter<TContext>[] parameters, string stepNameFormat, int stepNumber, Func<Type, ResultStatus> mapping)
        {
            _context = context;
            _action = action;
            _parameters = parameters;
            _stepNameFormat = stepNameFormat;
            _stepNumber = stepNumber;
            _mapping = mapping;
            _result = new StepResult(stepNumber, string.Format(CultureInfo.InvariantCulture, stepNameFormat, GetNotEvaluatedParameters()), ResultStatus.NotRun);
        }

        [DebuggerStepThrough]
        public void Invoke(IProgressNotifier progressNotifier, int totalCount)
        {
            try
            {
                InvokeStep(progressNotifier, totalCount);
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

        private void InvokeStep(IProgressNotifier progressNotifier, int totalCount)
        {
            var paramValues = EvaluateParameters();
            InvokeStep(paramValues, progressNotifier, totalCount);
        }

        [DebuggerStepThrough]
        private void InvokeStep(object[] paramValues, IProgressNotifier progressNotifier, int totalCount)
        {
            var stepName = string.Format(CultureInfo.InvariantCulture, _stepNameFormat, paramValues);
            _result = new StepResult(_stepNumber, stepName, ResultStatus.NotRun);

            progressNotifier.NotifyStepStart(stepName, _stepNumber, totalCount);
            MeasuredInvoke(paramValues);

            _result.SetStatus(ResultStatus.Passed);
        }

        [DebuggerStepThrough]
        private void MeasuredInvoke(object[] paramValues)
        {
            var watch = new Stopwatch();
            try
            {
                _result.SetExecutionStart(DateTimeOffset.UtcNow);
                watch.Start();
                _action(StepType.Default, _context, paramValues);
            }
            finally
            {
                _result.SetExecutionTime(watch.Elapsed);
            }
        }

        private object[] EvaluateParameters()
        {
            var result = new object[_parameters.Length];
            for (int index = 0; index < _parameters.Length; index++)
                result[index] = _parameters[index].Evaluate(_context);
            return result;
        }

        [DebuggerStepThrough]
        private object[] GetNotEvaluatedParameters()
        {
            return _parameters.Select(p => p.GetNotEvaluatedValue()).ToArray();
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}