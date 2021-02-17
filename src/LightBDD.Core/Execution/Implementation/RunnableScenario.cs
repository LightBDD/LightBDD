using LightBDD.Core.Dependencies;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.ExecutionContext.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    internal class RunnableScenario : IScenario, IRunnableScenario
    {
        private const int NotRunValue = 0;
        private const int RunValue = 1;
        private readonly RunnableScenarioContext _scenarioContext;
        private readonly IEnumerable<StepDescriptor> _stepDescriptors;
        private readonly ExecutionContextDescriptor _contextDescriptor;
        private readonly ScenarioResult _result;
        private readonly ExceptionCollector _exceptionCollector = new ExceptionCollector();
        private readonly Func<Task> _decoratedScenarioMethod;
        private IDependencyContainer _scope = NoDepencencyContainer.Instance;
        private Func<Exception, bool> _shouldAbortSubStepExecutionFn = ex => true;
        private RunnableStep[] _preparedSteps = Array.Empty<RunnableStep>();
        private int _alreadyRun = NotRunValue;
        public IScenarioInfo Info => _result.Info;
        public IDependencyResolver DependencyResolver => _scope;
        public object? Context { get; private set; }

        public RunnableScenario(RunnableScenarioContext scenarioContext, IScenarioInfo scenarioInfo,
            IEnumerable<StepDescriptor> stepDescriptors, ExecutionContextDescriptor contextDescriptor,
            IEnumerable<IScenarioDecorator> scenarioDecorators)
        {
            _scenarioContext = scenarioContext;
            _stepDescriptors = stepDescriptors;
            _contextDescriptor = contextDescriptor;
            _decoratedScenarioMethod = DecoratingExecutor.DecorateScenario(this, RunScenarioAsync, scenarioDecorators);
            _result = new ScenarioResult(scenarioInfo);
        }

        public async Task ExecuteAsync()
        {
            if (Interlocked.Exchange(ref _alreadyRun, RunValue) != NotRunValue)
                throw new InvalidOperationException("Scenario can be run only once");

            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                StartScenario();
                await _decoratedScenarioMethod.Invoke();
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

        private async Task RunScenarioAsync()
        {
            foreach (var step in _preparedSteps)
                await step.ExecuteAsync();
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
            ScenarioExecutionContext.SetCurrent(null);
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
            ScenarioExecutionContext.SetCurrent(CreateCurrentContext());
            PrepareSteps();
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
                return _scenarioContext.IntegrationContext.DependencyContainer.BeginScope(LifetimeScope.Scenario, _contextDescriptor.ScopeConfigurator);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Scenario container scope initialization failed: {e.Message}", e);
            }
        }

        private object? CreateExecutionContext()
        {
            try
            {
                return _contextDescriptor.ContextResolver(_scope);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Scenario context initialization failed: {e.Message}", e);
            }
        }

        private void PrepareSteps()
        {
            try
            {
                _preparedSteps = _scenarioContext.StepsProvider(_result.Info, _stepDescriptors, Context, _scope, string.Empty, ShouldAbortSubStepExecution);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Scenario steps initialization failed: {e.Message}", e);
            }
            if (_preparedSteps.Any(x => x.Result.ExecutionException != null))
                throw new InvalidOperationException("Scenario steps initialization failed.");
        }

        private bool ShouldAbortSubStepExecution(Exception ex) => _shouldAbortSubStepExecutionFn(ex);

        public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
        {
            _shouldAbortSubStepExecutionFn = shouldAbortExecutionFn ?? throw new ArgumentNullException(nameof(shouldAbortExecutionFn));
        }
    }
}