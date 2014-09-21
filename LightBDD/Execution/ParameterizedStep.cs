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
        private readonly Action<StepType, TContext, object[]> _action;
        private readonly Func<StepType, TContext, object>[] _parameters;
        private readonly string _stepNameFormat;
        private readonly int _stepNumber;
        private readonly Func<Type, ResultStatus> _mapping;
        private StepResult _result;
        public IStepResult GetResult() { return _result; }

        public ParameterizedStep(TContext context, Action<StepType, TContext, object[]> action, Func<StepType, TContext, object>[] parameters, string stepNameFormat, int stepNumber, Func<Type, ResultStatus> mapping)
        {
            _context = context;
            _action = action;
            _parameters = parameters;
            _stepNameFormat = stepNameFormat;
            _stepNumber = stepNumber;
            _mapping = mapping;
            _result = new StepResult(stepNumber, string.Format(stepNameFormat, GetNotEvaluatedParameters()), ResultStatus.NotRun);
        }

        public void Invoke(IProgressNotifier progressNotifier, int totalCount)
        {
            try
            {
                var paramValues = EvaluateParameters();
                var stepName = string.Format(_stepNameFormat, paramValues);
                _result = new StepResult(_stepNumber, stepName, ResultStatus.NotRun);

                progressNotifier.NotifyStepStart(stepName, _stepNumber, totalCount);
                _action(StepType.Default, _context, paramValues);

                _result.SetStatus(ResultStatus.Passed);
            }
            catch (Exception e)
            {
                _result.SetStatus(_mapping(e.GetType()), e.Message);
                throw;
            }
        }

        private object[] EvaluateParameters()
        {
            return _parameters.Select(p => p.Invoke(StepType.Default, _context)).ToArray();
        }

        private object[] GetNotEvaluatedParameters()
        {
            return _parameters.Select(p => (object)"<?>").ToArray();
        }
    }
}