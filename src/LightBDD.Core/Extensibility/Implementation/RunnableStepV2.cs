using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class RunnableStepV2 : IStep
    {
        private readonly RunnableStepContext _stepContext;
        private readonly StepFunc _invocation;
        private readonly MethodArgument[] _arguments;
        private readonly StepResult _result;
        private readonly ExceptionCollector _exceptionCollector = new ExceptionCollector();
        private Func<Exception, bool> _shouldAbortSubStepExecutionFn = _ => true;
        private IDependencyContainer _subStepScope;
        public IStepResult Result => _result;
        public IStepInfo Info => Result.Info;
        public IDependencyResolver DependencyResolver => _stepContext.Container;
        public object Context => _stepContext.Context;

        public RunnableStepV2(RunnableStepContext stepContext, StepInfo info, StepFunc invocation, MethodArgument[] arguments)
        {
            _stepContext = stepContext;
            _invocation = invocation;
            _arguments = arguments;
            _result = new StepResult(info);
            UpdateNameDetails();
        }

        public async Task ExecuteAsync()
        {
            var stepStartNotified = false;
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                StartStep();
                stepStartNotified = true;

                var result = await InvokeStepMethodAsync();
                await InvokeSubStepsAsync(result);

                UpdateStepStatus();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                StopStep(watch, stepStartNotified);
            }
            ProcessExceptions();
        }

        private async Task InvokeSubStepsAsync(IStepResultDescriptor result)
        {
            var subSteps = InitializeComposite(result);

            if (!subSteps.Any())
                return;

            try
            {
                foreach (var subStep in subSteps)
                    await subStep.ExecuteAsync();
            }
            finally
            {
                _result.SetSubSteps(subSteps.Select(s => s.Result).ToArray());
            }
        }

        private RunnableStepV2[] InitializeComposite(IStepResultDescriptor result)
        {
            if (!(result is CompositeStepResultDescriptor compositeDescriptor))
                return Arrays<RunnableStepV2>.Empty();

            _subStepScope = _stepContext.Container.BeginScope(compositeDescriptor.SubStepsContext.ScopeConfigurator);
            var subStepsContext = InstantiateSubStepsContext(compositeDescriptor.SubStepsContext, _subStepScope);
            try
            {
                return _stepContext.ProvideSteps(compositeDescriptor.SubSteps, subStepsContext, _subStepScope, $"{Info.GroupPrefix}{Info.Number}.", ShouldAbortSubStepExecution);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Sub-steps initialization failed: {e.Message}", e);
            }
        }

        private bool ShouldAbortSubStepExecution(Exception ex) => _shouldAbortSubStepExecutionFn(ex);

        private static object InstantiateSubStepsContext(ExecutionContextDescriptor contextDescriptor, IDependencyContainer container)
        {
            try
            {
                return contextDescriptor.ContextResolver(container);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Sub-steps context initialization failed: {e.Message}", e);
            }
        }

        private async Task<IStepResultDescriptor> InvokeStepMethodAsync()
        {
            IStepResultDescriptor result;
            var ctx = AsyncStepSynchronizationContext.InstallNew();
            try
            {
                result = await _invocation.Invoke(Context, PrepareParameters());
                VerifyParameters();
            }
            catch (Exception e)
            {
                if (ScenarioExecutionException.TryWrap(e, out var wrapped))
                    throw wrapped;
                throw;
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
            var results = new List<IParameterResult>();
            foreach (var argument in _arguments)
            {
                if (argument.Value is IComplexParameter complex)
                    results.Add(new ParameterResult(argument.RawName, complex.Details));
            }

            _result.SetParameters(results);

            var errors = results
                .Where(x => x.Details.VerificationStatus > ParameterVerificationStatus.Success)
                .Select(FormatErrorMessage)
                .ToArray();

            if (!errors.Any())
                return;

            throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
        }
        private static string FormatErrorMessage(IParameterResult result)
        {
            return $"Parameter '{result.Name}' verification failed: {result.Details.VerificationMessage?.Replace(Environment.NewLine, Environment.NewLine + "\t") ?? string.Empty}";
        }

        private object[] PrepareParameters()
        {
            return _arguments.Select(p => p.Value).ToArray();
        }

        private void UpdateStepStatus()
        {
            _result.SetStatus(_result.GetSubSteps().GetMostSevereOrNull()?.Status ?? ExecutionStatus.Passed);
        }

        private void ProcessExceptions()
        {
            var exception = _exceptionCollector.CollectFor(_result.Status, _result.GetSubSteps());
            if (exception == null)
                return;

            _result.UpdateException(exception);
            if (_stepContext.ShouldAbortSubStepExecution(exception))
                throw new StepExecutionException(exception, _result.Status);
        }

        private void StartStep()
        {
            EvaluateParameters();
            _stepContext.ProgressNotifier.NotifyStepStart(_result.Info);
        }

        private void StopStep(ExecutionTimeWatch watch, bool stepStartNotified)
        {
            _subStepScope?.Dispose();
            watch.Stop();
            _result.SetExecutionTime(watch.GetTime());
            _result.IncludeSubStepDetails();
            if (stepStartNotified)
                _stepContext.ProgressNotifier.NotifyStepFinished(_result);
        }

        private void HandleException(Exception exception)
        {
            switch (exception)
            {
                case StepExecutionException e:
                    _result.SetStatus(e.StepStatus);
                    break;
                case ScenarioExecutionException e when e.InnerException is StepBypassException:
                    _result.SetStatus(ExecutionStatus.Bypassed, exception.InnerException.Message);
                    break;
                case ScenarioExecutionException e:
                    _stepContext.ExceptionProcessor.UpdateResultsWithException(_result.SetStatus, e.InnerException);
                    _exceptionCollector.Capture(e);
                    break;
                default:
                    _stepContext.ExceptionProcessor.UpdateResultsWithException(_result.SetStatus, exception);
                    _exceptionCollector.Capture(exception);
                    break;
            }
        }

        private void EvaluateParameters()
        {
            foreach (var parameter in _arguments)
                parameter.Evaluate(Context);
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

        public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
        {
            _shouldAbortSubStepExecutionFn = shouldAbortExecutionFn ?? throw new ArgumentNullException(nameof(shouldAbortExecutionFn));
        }

        public void Comment(string comment)
        {
            _result.AddComment(comment);
            _stepContext.ProgressNotifier.NotifyStepComment(_result.Info, comment);
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}