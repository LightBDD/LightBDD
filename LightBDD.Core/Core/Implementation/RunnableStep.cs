using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Implementation
{
    internal class RunnableStep
    {
        private readonly Func<object, object[], Task> _stepInvocation;
        private readonly StepParameter[] _parameters;
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private readonly StepResult _result;
        public IStepResult Result => _result;

        public RunnableStep(StepInfo stepInfo, Func<object, object[], Task> stepInvocation, StepParameter[] parameters, Func<Exception, ExecutionStatus> exceptionToStatusMapper)
        {
            _result = new StepResult(stepInfo);
            _stepInvocation = stepInvocation;
            _parameters = parameters;
            _exceptionToStatusMapper = exceptionToStatusMapper;
            UpdateNameDetails();
        }

        private void UpdateNameDetails()
        {
            if (!_parameters.Any())
                return;

            _result.UpdateName(_parameters.Select(p => p.FormatNameParameter()).ToArray());
        }

        public async Task Invoke(object context)
        {
            try
            {
                EvaluateParameters(context);
                await TimeMeasuredInvoke(context);
                _result.SetStatus(ExecutionStatus.Passed);
            }
            catch (StepBypassException e)
            {
                _result.SetStatus(ExecutionStatus.Bypassed, e.Message);
            }
            catch (Exception e)
            {
                _result.SetStatus(_exceptionToStatusMapper(e), e.Message);
                throw;
            }
        }

        private async Task TimeMeasuredInvoke(object context)
        {
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                await _stepInvocation.Invoke(context, PrepareParameters());
            }
            finally
            {
                _result.SetExecutionTime(watch.GetTime());
            }
        }

        private void EvaluateParameters(object context)
        {
            foreach (var parameter in _parameters)
                parameter.Evaluate(context);
            UpdateNameDetails();
        }

        private object[] PrepareParameters()
        {
            return _parameters.Select(p => p.Value).ToArray();
        }
    }
}