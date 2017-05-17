using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class RunnableStep : IStep
    {
        private readonly Func<object, object[], Task<RunnableStepResult>> _stepInvocation;
        private readonly MethodArgument[] _arguments;
        private readonly ExceptionProcessor _exceptionProcessor;
        private readonly IScenarioProgressNotifier _progressNotifier;
        private readonly ExtendableExecutor _extendableExecutor;
        private readonly object _scenarioContext;
        private readonly StepResult _result;
        public IStepResult Result => _result;
        public IStepInfo Info => Result.Info;

        public RunnableStep(StepInfo stepInfo, Func<object, object[], Task<RunnableStepResult>> stepInvocation, MethodArgument[] arguments, ExceptionProcessor exceptionProcessor, IScenarioProgressNotifier progressNotifier, ExtendableExecutor extendableExecutor, object scenarioContext)
        {
            _result = new StepResult(stepInfo);
            _stepInvocation = stepInvocation;
            _arguments = arguments;
            _exceptionProcessor = exceptionProcessor;
            _progressNotifier = progressNotifier;
            _extendableExecutor = extendableExecutor;
            _scenarioContext = scenarioContext;
            UpdateNameDetails();
        }

        private void UpdateNameDetails()
        {
            if (!_arguments.Any())
                return;

            _result.UpdateName(_arguments.Select(FormatStepParameter).ToArray());
        }

        private INameParameterInfo FormatStepParameter(MethodArgument p)
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

        public async Task RunAsync()
        {
            bool stepStartNotified = false;
            try
            {
                EvaluateParameters();
                _progressNotifier.NotifyStepStart(_result.Info);
                stepStartNotified = true;

                await _extendableExecutor.ExecuteStepAsync(this, TimeMeasuredInvokeAsync);
                _result.SetStatus(_result.GetSubSteps().GetMostSevereOrNull()?.Status ?? ExecutionStatus.Passed);
            }
            catch (StepBypassException exception)
            {
                _result.SetStatus(ExecutionStatus.Bypassed, exception.Message);
            }
            catch (StepAbortedException e)
            {
                _result.SetStatus(e.StepStatus);
                throw;
            }
            catch (Exception exception)
            {
                var executionStatus = _exceptionProcessor.UpdateResultsWithException(_result, exception);
                throw new StepAbortedException(exception, executionStatus);
            }
            finally
            {
                _result.IncludeSubStepDetails();
                if (stepStartNotified)
                    _progressNotifier.NotifyStepFinished(_result);
            }
        }

        private async Task TimeMeasuredInvokeAsync()
        {
            var watch = ExecutionTimeWatch.StartNew();

            try
            {
                var result = await InvokeStepMethodAsync();
                await InvokeSubStepsAsync(result.SubSteps);
            }
            finally
            {
                _result.SetExecutionTime(watch.GetTime());
            }
        }

        private async Task InvokeSubStepsAsync(RunnableStep[] subSteps)
        {
            try
            {
                foreach (var subStep in subSteps)
                    await subStep.RunAsync();
            }
            finally
            {
                _result.SetSubSteps(subSteps.Select(s => s.Result).ToArray());
            }
        }

        private async Task<RunnableStepResult> InvokeStepMethodAsync()
        {
            RunnableStepResult result;
            var ctx = AsyncStepSynchronizationContext.InstallNew();
            try
            {
                result = await _stepInvocation.Invoke(_scenarioContext, PrepareParameters());
            }
            finally
            {
                ctx.RestoreOriginal();
                await ctx.WaitForTasksAsync();
            }
            return result;
        }

        private void EvaluateParameters()
        {
            foreach (var parameter in _arguments)
                parameter.Evaluate(_scenarioContext);
            UpdateNameDetails();
        }

        private object[] PrepareParameters()
        {
            return _arguments.Select(p => p.Value).ToArray();
        }

        public void Comment(string comment)
        {
            _result.AddComment(comment);
            _progressNotifier.NotifyStepComment(_result.Info, comment);
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}