using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting.Parameters;
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
        private readonly DecoratingExecutor _decoratingExecutor;
        private readonly object _scenarioContext;
        private readonly IEnumerable<IStepDecorator> _stepDecorators;
        private readonly StepResult _result;
        private Func<Exception, bool> _shouldAbortSubStepExecutionFn = ex => true;
        public IStepResult Result => _result;
        public IStepInfo Info => Result.Info;

        [DebuggerStepThrough]
        public RunnableStep(StepInfo stepInfo, Func<object, object[], Task<RunnableStepResult>> stepInvocation, MethodArgument[] arguments, ExceptionProcessor exceptionProcessor, IScenarioProgressNotifier progressNotifier, DecoratingExecutor decoratingExecutor, object scenarioContext, IEnumerable<IStepDecorator> stepDecorators)
        {
            _result = new StepResult(stepInfo);
            _stepInvocation = stepInvocation;
            _arguments = arguments;
            _exceptionProcessor = exceptionProcessor;
            _progressNotifier = progressNotifier;
            _decoratingExecutor = decoratingExecutor;
            _scenarioContext = scenarioContext;
            _stepDecorators = stepDecorators;
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
            var exceptionCollector = new ExceptionCollector();
            var stepStartNotified = false;
            try
            {
                EvaluateParameters();
                _progressNotifier.NotifyStepStart(_result.Info);
                stepStartNotified = true;

                await TimeMeasuredInvokeAsync();
                _result.SetStatus(_result.GetSubSteps().GetMostSevereOrNull()?.Status ?? ExecutionStatus.Passed);
            }
            catch (StepBypassException exception)
            {
                _result.SetStatus(ExecutionStatus.Bypassed, exception.Message);
            }
            catch (StepExecutionException e)
            {
                _result.SetStatus(e.StepStatus);
            }
            catch (Exception exception)
            {
                _exceptionProcessor.UpdateResultsWithException(_result.SetStatus, exception);
                exceptionCollector.Capture(exception);
            }
            finally
            {
                _result.IncludeSubStepDetails();
                if (stepStartNotified)
                    _progressNotifier.NotifyStepFinished(_result);
            }
            ProcessExceptions(exceptionCollector);
        }

        [DebuggerStepThrough]
        private void ProcessExceptions(ExceptionCollector exceptionCollector)
        {
            var exception = exceptionCollector.CollectFor(_result.Status, _result.GetSubSteps());
            if (exception == null)
                return;

            _result.UpdateException(exception);

            throw new StepExecutionException(exception, _result.Status);
        }

        [DebuggerStepThrough]
        private async Task TimeMeasuredInvokeAsync()
        {
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                await _decoratingExecutor.ExecuteStepAsync(this, InvokeStepAsync, _stepDecorators);
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
                    await InvokeSubStepAsync(subStep);
            }
            finally
            {
                _result.SetSubSteps(subSteps.Select(s => s.Result).ToArray());
            }
        }

        [DebuggerStepThrough]
        private async Task InvokeSubStepAsync(RunnableStep subStep)
        {
            try
            {
                await subStep.RunAsync();
            }
            catch (Exception ex)
            {
                if (_shouldAbortSubStepExecutionFn(ex))
                    throw;
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
                VerifyParameters();
            }
            finally
            {
                UpdateNameDetails();
                ctx.RestoreOriginal();
                await ctx.WaitForTasksAsync();
            }
            return result;
        }

        private void VerifyParameters()
        {
            if (!_arguments.All(a => (a.Value as IVerifiableParameter)?.IsValid ?? true))
                throw new ArgumentException("Argument validation failed");
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
        public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
        {
            _shouldAbortSubStepExecutionFn = shouldAbortExecutionFn ?? throw new ArgumentNullException(nameof(shouldAbortExecutionFn));
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return _result.ToString();
        }
    }
}