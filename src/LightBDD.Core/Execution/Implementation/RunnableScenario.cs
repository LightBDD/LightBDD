using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    internal class RunnableScenario : IScenario
    {
        private readonly ScenarioInfo _info;
        private readonly Func<DecoratingExecutor, object, IDependencyContainer, RunnableStep[]> _stepsProvider;
        private readonly ExecutionContextDescriptor _contextDescriptor;
        private readonly IScenarioProgressNotifier _progressNotifier;
        private readonly DecoratingExecutor _decoratingExecutor;
        private readonly IEnumerable<IScenarioDecorator> _scenarioDecorators;
        private readonly ExceptionProcessor _exceptionProcessor;
        private readonly IDependencyContainer _container;
        private readonly ScenarioResult _result;
        private RunnableStep[] _preparedSteps = new RunnableStep[0];
        private object _scenarioContext;
        private Func<Exception, bool> _shouldAbortSubStepExecutionFn = ex => true;
        private IDependencyContainer _scope;

        [DebuggerStepThrough]
        public RunnableScenario(ScenarioInfo scenario, Func<DecoratingExecutor, object, IDependencyContainer, RunnableStep[]> stepsProvider,
            ExecutionContextDescriptor contextDescriptor, IScenarioProgressNotifier progressNotifier,
            DecoratingExecutor decoratingExecutor, IEnumerable<IScenarioDecorator> scenarioDecorators,
            ExceptionProcessor exceptionProcessor, IDependencyContainer container)
        {
            _info = scenario;
            _stepsProvider = stepsProvider;
            _contextDescriptor = contextDescriptor;
            _progressNotifier = progressNotifier;
            _decoratingExecutor = decoratingExecutor;
            _scenarioDecorators = scenarioDecorators;
            _exceptionProcessor = exceptionProcessor;
            _container = container;
            _result = new ScenarioResult(_info);
        }

        public IScenarioResult Result => _result;

        private async Task RunScenarioAsync()
        {
            foreach (var step in _preparedSteps)
                await RunStepAsync(step);
        }

        [DebuggerStepThrough]
        private async Task RunStepAsync(RunnableStep step)
        {
            try
            {
                await step.RunAsync();
            }
            catch (Exception ex)
            {
                if (_shouldAbortSubStepExecutionFn(ex))
                    throw;
            }
        }

        [DebuggerStepThrough]
        private void InitializeScenario()
        {
            _scope = CreateContainerScope();
            _scenarioContext = CreateExecutionContext();
            _preparedSteps = PrepareSteps();
        }

        private IDependencyContainer CreateContainerScope()
        {
            try
            {
                return _container.BeginScope(_contextDescriptor.ScopeConfigurer);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Container scope initialization failed: {e.Message}", e);
            }
        }

        [DebuggerStepThrough]
        public async Task RunAsync()
        {
            var exceptionCollector = new ExceptionCollector();
            _progressNotifier.NotifyScenarioStart(_info);
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                InitializeScenario();
                await _decoratingExecutor.ExecuteScenarioAsync(this, RunScenarioAsync, _scenarioDecorators);
            }
            catch (StepExecutionException ex)
            {
                _result.UpdateScenarioResult(ex.StepStatus);
            }
            catch (ScenarioExecutionException ex) when (ex.InnerException is StepBypassException)
            {
                _result.UpdateScenarioResult(ExecutionStatus.Bypassed, ex.InnerException.Message);
            }
            catch (ScenarioExecutionException ex)
            {
                _exceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, ex.InnerException);
                exceptionCollector.Capture(ex);
            }
            catch (Exception ex)
            {
                _exceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, ex);
                exceptionCollector.Capture(ex);
            }
            finally
            {
                DisposeScope(exceptionCollector);
                watch.Stop();

                _result.UpdateResult(
                    _preparedSteps.Select(s => s.Result).ToArray(),
                    watch.GetTime());

                _progressNotifier.NotifyScenarioFinished(Result);
            }

            ProcessExceptions(exceptionCollector);
        }

        [DebuggerStepThrough]
        private void DisposeScope(ExceptionCollector exceptionCollector)
        {
            try
            {
                _scope.Dispose();
            }
            catch (Exception exception)
            {
                _exceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, exception);
                exceptionCollector.Capture(exception);
            }
        }

        [DebuggerStepThrough]
        private void ProcessExceptions(ExceptionCollector exceptionCollector)
        {
            var exception = exceptionCollector.CollectFor(_result.Status, _result.GetSteps());
            if (exception == null)
                return;

            _result.UpdateException(exception);

            throw new ScenarioExecutionException(exception);
        }

        [DebuggerStepThrough]
        private RunnableStep[] PrepareSteps()
        {
            try
            {
                return _stepsProvider.Invoke(_decoratingExecutor, _scenarioContext, _scope);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Step initialization failed: {e.Message}", e);
            }
        }

        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
        public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
        {
            _shouldAbortSubStepExecutionFn = shouldAbortExecutionFn ?? throw new ArgumentNullException(nameof(shouldAbortExecutionFn));
        }

        public IScenarioInfo Info => _info;
    }
}