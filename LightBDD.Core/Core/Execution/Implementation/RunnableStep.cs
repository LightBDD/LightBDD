using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Execution.Implementation
{
    internal class RunnableStep : IStep
    {
        private readonly Func<object, object[], Task> _stepInvocation;
        private readonly StepParameter[] _parameters;
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private readonly IScenarioProgressNotifier _progressNotifier;
        private readonly StepResult _result;
        public IStepResult Result => _result;
        public IStepInfo Info => Result.Info;

        public RunnableStep(StepInfo stepInfo, Func<object, object[], Task> stepInvocation, StepParameter[] parameters, Func<Exception, ExecutionStatus> exceptionToStatusMapper, IScenarioProgressNotifier progressNotifier)
        {
            _result = new StepResult(stepInfo);
            _stepInvocation = stepInvocation;
            _parameters = parameters;
            _exceptionToStatusMapper = exceptionToStatusMapper;
            _progressNotifier = progressNotifier;
            UpdateNameDetails();
        }

        private void UpdateNameDetails()
        {
            if (!_parameters.Any())
                return;

            _result.UpdateName(_parameters.Select(FormatStepParameter).ToArray());
        }

        private INameParameterInfo FormatStepParameter(StepParameter p)
        {
            try
            {
                return p.FormatNameParameter();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Unable to format '{p.RawName}' parameter of step '{_result.Info}': {e.Message}");
            }
        }

        public async Task Invoke(IExtendableExecutor extendableExecutor, object context)
        {
            bool stepStartNotified = false;
            try
            {
                EvaluateParameters(context);
                _progressNotifier.NotifyStepStart(_result.Info);
                stepStartNotified = true;

                await extendableExecutor.ExecuteStep(this, () => TimeMeasuredInvoke(context));
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
            finally
            {
                if (stepStartNotified)
                    _progressNotifier.NotifyStepFinished(_result);
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

        public void Comment(string comment)
        {
            _result.AddComment(comment);
            _progressNotifier.NotifyStepComment(_result.Info, comment);
        }
    }
}