using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution;
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
    internal class RunnableStep : IStep
    {
        private readonly Func<object, object[], Task<RunnableStepResult>> _stepInvocation;
        private readonly MethodArgument[] _arguments;
        private readonly ExceptionProcessor _exceptionProcessor;
        private readonly IScenarioProgressNotifier _progressNotifier;
        private readonly ExtendableExecutor _extendableExecutor;
        private readonly object _scenarioContext;
        private readonly IEnumerable<IStepExecutionExtension> _stepExecutionExtensions;
        private readonly StepResult _result;
        public IStepResult Result => _result;
        public IStepInfo Info => Result.Info;

        [DebuggerStepThrough]
        public RunnableStep(StepInfo stepInfo, Func<object, object[], Task<RunnableStepResult>> stepInvocation, MethodArgument[] arguments, ExceptionProcessor exceptionProcessor, IScenarioProgressNotifier progressNotifier, ExtendableExecutor extendableExecutor, object scenarioContext, IEnumerable<IStepExecutionExtension> stepExecutionExtensions)
        {
            _result = new StepResult(stepInfo);
            _stepInvocation = stepInvocation;
            _arguments = arguments;
            _exceptionProcessor = exceptionProcessor;
            _progressNotifier = progressNotifier;
            _extendableExecutor = extendableExecutor;
            _scenarioContext = scenarioContext;
            _stepExecutionExtensions = stepExecutionExtensions;
            UpdateNameDetails();
        }

        [DebuggerStepThrough]
        private void UpdateNameDetails()
        {
            if (!_arguments.Any())
                return;

            _result.UpdateName(_arguments.Select(FormatStepParameter).ToArray());
        }

        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
        public async Task RunAsync()
        {
            bool stepStartNotified = false;
            try
            {
                EvaluateParameters();
                _progressNotifier.NotifyStepStart(_result.Info);
                stepStartNotified = true;

                await _extendableExecutor.ExecuteStepAsync(this, TimeMeasuredInvokeAsync, _stepExecutionExtensions);
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

        [DebuggerStepThrough]
        private async Task TimeMeasuredInvokeAsync()
        {
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                await InvokeStepAsync();
            }
            finally
            {
                _result.SetExecutionTime(watch.GetTime());
            }
        }

        private async Task InvokeStepAsync()
        {
            var result = await InvokeStepMethodAsync();
            if (result.SubSteps.Any())
                await InvokeSubStepsAsync(result.SubSteps);
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

        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
        private void EvaluateParameters()
        {
            foreach (var parameter in _arguments)
                parameter.Evaluate(_scenarioContext);
            UpdateNameDetails();
        }

        [DebuggerStepThrough]
        private object[] PrepareParameters()
        {
            return _arguments.Select(p => p.Value).ToArray();
        }

        [DebuggerStepThrough]
        public void Comment(string comment)
        {
            _result.AddComment(comment);
            _progressNotifier.NotifyStepComment(_result.Info, comment);
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return _result.ToString();
        }
    }
}