using System;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD.Execution
{
    internal class ParameterizedStep<TContext> : IStep
    {
        private readonly TContext _context;
        private readonly Action<TContext, object[]> _action;
        private readonly Func<TContext, object>[] _parameters;
        private readonly string _stepNameFormat;
        private readonly int _stepNumber;
        private readonly Func<Type, ResultStatus> _mapping;
        private StepResult _result;
        public IStepResult GetResult() { return _result; }

        public ParameterizedStep(TContext context, Action<TContext, object[]> action, Func<TContext, object>[] parameters, string stepNameFormat, int stepNumber, Func<Type, ResultStatus> mapping)
        {
            _context = context;
            _action = action;
            _parameters = parameters;
            _stepNameFormat = stepNameFormat;
            _stepNumber = stepNumber;
            _mapping = mapping;
            _result = new StepResult(stepNumber, string.Format(stepNameFormat, parameters.Select(p => (object)"<?>").ToArray()), ResultStatus.NotRun);
        }

        public void Invoke(IProgressNotifier progressNotifier, int totalCount)
        {
            try
            {
                var paramValues = _parameters.Select(p => p.Invoke(_context)).ToArray();
                var stepName = string.Format(_stepNameFormat, paramValues);
                _result = new StepResult(_stepNumber, stepName, ResultStatus.NotRun);

                progressNotifier.NotifyStepStart(stepName, _stepNumber, totalCount);
                _action(_context, paramValues);

                _result.SetStatus(ResultStatus.Passed);
            }
            catch (Exception e)
            {
                _result.SetStatus(_mapping(e.GetType()), e.Message);
                throw;
            }
        }
    }
}