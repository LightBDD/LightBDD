using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    internal class RunnableScenario
    {
        private readonly ScenarioInfo _scenario;
        private readonly Func<ExtendableExecutor, object, RunnableStep[]> _stepsProvider;
        private readonly Func<object> _contextProvider;
        private readonly IScenarioProgressNotifier _progressNotifier;
        private readonly ExtendableExecutor _extendableExecutor;
        private readonly IEnumerable<IScenarioExecutionExtension> _scenarioExecutionExtensions;
        private readonly ExceptionProcessor _exceptionProcessor;
        private readonly ScenarioResult _result;
        private Exception _scenarioInitializationException;
        private RunnableStep[] _preparedSteps = new RunnableStep[0];
        private object _scenarioContext;

        [DebuggerStepThrough]
        public RunnableScenario(ScenarioInfo scenario, Func<ExtendableExecutor, object, RunnableStep[]> stepsProvider, Func<object> contextProvider, IScenarioProgressNotifier progressNotifier, ExtendableExecutor extendableExecutor, IEnumerable<IScenarioExecutionExtension> scenarioExecutionExtensions, ExceptionProcessor exceptionProcessor)
        {
            _scenario = scenario;
            _stepsProvider = stepsProvider;
            _contextProvider = contextProvider;
            _progressNotifier = progressNotifier;
            _extendableExecutor = extendableExecutor;
            _scenarioExecutionExtensions = scenarioExecutionExtensions;
            _exceptionProcessor = exceptionProcessor;
            _result = new ScenarioResult(_scenario);
        }

        public IScenarioResult Result => _result;

        private async Task RunScenarioAsync()
        {
            foreach (var step in _preparedSteps)
                await step.RunAsync();
        }

        [DebuggerStepThrough]
        private void InitializeScenario()
        {
            try
            {
                _scenarioContext = CreateExecutionContext();
                _preparedSteps = PrepareSteps();
            }
            catch (Exception e)
            {
                _scenarioInitializationException = e;
                throw;
            }
        }

        [DebuggerStepThrough]
        public async Task RunAsync()
        {
            _progressNotifier.NotifyScenarioStart(_scenario);
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                InitializeScenario();
                await _extendableExecutor.ExecuteScenarioAsync(_scenario, RunScenarioAsync, _scenarioExecutionExtensions);
            }
            catch (StepBypassException ex)
            {
                _result.UpdateScenarioResult(ExecutionStatus.Bypassed, ex.Message);
            }
            catch (StepAbortedException ex)
            {
                ex.RethrowOriginalException();
            }
            catch (Exception ex)
            {
                _exceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, ex);
                throw;
            }
            finally
            {
                watch.Stop();

                _result.UpdateResult(
                    _preparedSteps.Select(s => s.Result).ToArray(),
                    watch.GetTime(),
                    _scenarioInitializationException);

                _progressNotifier.NotifyScenarioFinished(Result);
            }
        }

        [DebuggerStepThrough]
        private RunnableStep[] PrepareSteps()
        {
            try
            {
                return _stepsProvider.Invoke(_extendableExecutor, _scenarioContext);
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
                return _contextProvider.Invoke();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Context initialization failed: {e.Message}", e);
            }
        }
    }
}