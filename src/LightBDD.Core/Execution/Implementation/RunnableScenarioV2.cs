using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.ExecutionContext.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    internal class RunnableScenarioV2 : IScenario
    {
        private readonly RunnableScenarioContext _scenarioContext;
        private readonly IEnumerable<StepDescriptor> _stepDescriptors;
        private readonly ExecutionContextDescriptor _contextDescriptor;
        private readonly ScenarioResult _result;
        private readonly ExceptionCollector _exceptionCollector = new ExceptionCollector();
        private IDependencyContainer _scope;
        private Func<Exception, bool> _shouldAbortSubStepExecutionFn = ex => true;
        private RunnableStepV2[] _preparedSteps;
        public IScenarioInfo Info => _result.Info;
        public IDependencyResolver DependencyResolver => _scope;
        public object Context { get; private set; }

        public RunnableScenarioV2(RunnableScenarioContext scenarioContext, ScenarioInfo scenarioInfo, IEnumerable<StepDescriptor> stepDescriptors, ExecutionContextDescriptor contextDescriptor)
        {
            _scenarioContext = scenarioContext;
            _stepDescriptors = stepDescriptors;
            _contextDescriptor = contextDescriptor;
            _result = new ScenarioResult(scenarioInfo);
        }

        public async Task ExecuteAsync()
        {
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                StartScenario();

                foreach (var step in _preparedSteps)
                    await step.ExecuteAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                StopScenario(watch);
            }

            ProcessExceptions();
        }

        private void ProcessExceptions()
        {
            var exception = _exceptionCollector.CollectFor(_result.Status, _result.GetSteps());
            if (exception == null)
                return;

            _result.UpdateException(exception);
            throw new ScenarioExecutionException(exception);
        }

        private void StopScenario(ExecutionTimeWatch watch)
        {
            ScenarioExecutionContext.Current = null;
            DisposeScope();
            watch.Stop();
            _result.UpdateResult(
                _preparedSteps.Select(s => s.Result).ToArray(),
                watch.GetTime());

            _scenarioContext.ProgressNotifier.NotifyScenarioFinished(_result);
            _scenarioContext.OnScenarioFinished?.Invoke(_result);
        }

        private void HandleException(Exception exception)
        {
            switch (exception)
            {
                case StepExecutionException stepException:
                    _result.UpdateScenarioResult(stepException.StepStatus);
                    break;
                case ScenarioExecutionException scenarioException when scenarioException.InnerException is StepBypassException:
                    _result.UpdateScenarioResult(ExecutionStatus.Bypassed, scenarioException.InnerException.Message);
                    break;
                case ScenarioExecutionException scenarioException:
                    _scenarioContext.ExceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, scenarioException.InnerException);
                    _exceptionCollector.Capture(scenarioException);
                    break;
                default:
                    _scenarioContext.ExceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, exception);
                    _exceptionCollector.Capture(exception);
                    break;
            }
        }

        private void DisposeScope()
        {
            try
            {
                _scope.Dispose();
            }
            catch (Exception exception)
            {
                _scenarioContext.ExceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, exception);
                _exceptionCollector.Capture(exception);
            }
        }

        private void StartScenario()
        {
            _scenarioContext.ProgressNotifier.NotifyScenarioStart(Info);
            _scope = CreateContainerScope();
            Context = CreateExecutionContext();
            _preparedSteps = PrepareSteps();
            ScenarioExecutionContext.Current = CreateCurrentContext();
        }

        private ScenarioExecutionContext CreateCurrentContext()
        {
            var executionContext = new ScenarioExecutionContext();
            executionContext.Get<CurrentScenarioProperty>().Scenario = this;
            return executionContext;
        }

        private IDependencyContainer CreateContainerScope()
        {
            try
            {
                return _scenarioContext.IntegrationContext.DependencyContainer.BeginScope(_contextDescriptor.ScopeConfigurator);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Container scope initialization failed: {e.Message}", e);
            }
        }

        private object CreateExecutionContext()
        {
            try
            {
                return _contextDescriptor.ContextResolver(_scope);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Context initialization failed: {e.Message}", e);
            }
        }

        private RunnableStepV2[] PrepareSteps()
        {
            try
            {
                return _scenarioContext.StepsProvider(_stepDescriptors, Context, _scope, string.Empty, ShouldAbortSubStepExecution);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Step initialization failed: {e.Message}", e);
            }
        }

        private bool ShouldAbortSubStepExecution(Exception ex) => _shouldAbortSubStepExecutionFn(ex);

        public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
        {
            _shouldAbortSubStepExecutionFn = shouldAbortExecutionFn ?? throw new ArgumentNullException(nameof(shouldAbortExecutionFn));
        }
    }
}