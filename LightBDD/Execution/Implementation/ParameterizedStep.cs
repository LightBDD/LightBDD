using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LightBDD.Execution.Exceptions;
using LightBDD.Execution.Implementation.Parameters;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD.Execution.Implementation
{
    internal class ParameterizedStep<TContext> : IStep
    {
        private readonly MethodInfo _stepMethod;
        private readonly TContext _context;
        private readonly Action<StepType, TContext, object[]> _action;
        private readonly IStepParameter<TContext>[] _parameters;
        private readonly string _formattedStepTypeName;
        private readonly string _stepNameFormat;
        private readonly int _stepNumber;
        private readonly Func<Type, ResultStatus> _mapping;
        private StepResult _result;
        public IStepResult GetResult() { return _result; }

        [DebuggerStepThrough]
        public ParameterizedStep(MethodInfo stepMethod, TContext context, Action<StepType, TContext, object[]> action, IStepParameter<TContext>[] parameters, string formattedStepTypeName, string stepNameFormat, int stepNumber, Func<Type, ResultStatus> mapping)
        {
            _stepMethod = stepMethod;
            _context = context;
            _action = action;
            _parameters = parameters;
            _formattedStepTypeName = formattedStepTypeName;
            _stepNameFormat = stepNameFormat;
            _stepNumber = stepNumber;
            _mapping = mapping;
            _result = new StepResult(stepNumber, new StepName(stepNameFormat, formattedStepTypeName, GetParameterDetails()), ResultStatus.NotRun);
        }

        [DebuggerStepThrough]
        public void Invoke(IProgressNotifier progressNotifier, int totalCount)
        {
            try
            {
                InvokeStep(progressNotifier, totalCount);
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
                progressNotifier.NotifyStepFinished(_result, totalCount);
            }
        }

        private void InvokeStep(IProgressNotifier progressNotifier, int totalCount)
        {
            EvaluateParameters();
            InvokeStepWithEvaluatedParameters(progressNotifier, totalCount);
        }

        [DebuggerStepThrough]
        private void InvokeStepWithEvaluatedParameters(IProgressNotifier progressNotifier, int totalCount)
        {
            _result = new StepResult(_stepNumber, new StepName(_stepNameFormat, _formattedStepTypeName, GetParameterDetails()), ResultStatus.NotRun);

            progressNotifier.NotifyStepStart(_result.Name, _stepNumber, totalCount);
            MeasuredInvoke(_parameters.Select(p => p.Value).ToArray());

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

        private void EvaluateParameters()
        {
            foreach (var parameter in _parameters)
                parameter.Evaluate(_context);
        }

        [DebuggerStepThrough]
        private IStepParameter[] GetParameterDetails()
        {
            return _parameters.Select(CreateStepParameterDetails).ToArray();
        }

        [DebuggerStepThrough]
        private IStepParameter CreateStepParameterDetails(IStepParameter<TContext> parameter)
        {
            try
            {
                return new StepParameter(parameter.IsEvaluated, parameter.Format());
            }
            catch (Exception e)
            {
                throw new ArgumentException(string.Format("Unable to format '{0}' parameter of step {1} '{2}': {3}", parameter.Name, _stepNumber, _stepMethod.Name, e.Message), e);
            }
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}