using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
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
        private readonly Func<ExtendableExecutor, object, RunnableStep[]> _stepsProvider;
        private readonly Func<object> _contextProvider;
        private readonly IScenarioProgressNotifier _progressNotifier;
        private readonly ExtendableExecutor _extendableExecutor;
        private readonly IEnumerable<IScenarioExtension> _scenarioExtensions;
        private readonly ExceptionProcessor _exceptionProcessor;
        private readonly ScenarioResult _result;
        private Exception _scenarioInitializationException;
        private RunnableStep[] _preparedSteps = new RunnableStep[0];
        private object _scenarioContext;
        private Func<Exception, bool> _shouldAbortSubStepExecutionFn = ex => true;

        [DebuggerStepThrough]
        public RunnableScenario(ScenarioInfo scenario, Func<ExtendableExecutor, object, RunnableStep[]> stepsProvider, Func<object> contextProvider, IScenarioProgressNotifier progressNotifier, ExtendableExecutor extendableExecutor, IEnumerable<IScenarioExtension> scenarioExtensions, ExceptionProcessor exceptionProcessor)
        {
            _info = scenario;
            _stepsProvider = stepsProvider;
            _contextProvider = contextProvider;
            _progressNotifier = progressNotifier;
            _extendableExecutor = extendableExecutor;
            _scenarioExtensions = scenarioExtensions;
            _exceptionProcessor = exceptionProcessor;
            _result = new ScenarioResult(_info);
        }

        public IScenarioResult Result => _result;

        private async Task RunScenarioAsync()
        {
            foreach (var step in _preparedSteps)
                await RunStepAsync(step);
        }

        private  async Task RunStepAsync(RunnableStep step)
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
            _progressNotifier.NotifyScenarioStart(_info);
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                InitializeScenario();
                await _extendableExecutor.ExecuteScenarioAsync(this, RunScenarioAsync, _scenarioExtensions);
            }
            catch (StepBypassException ex)
            {
                _result.UpdateScenarioResult(ExecutionStatus.Bypassed, ex.Message);
            }
            catch (StepExecutionException ex)
            {
                _result.UpdateScenarioResult(ex.StepStatus);
            }
            catch (Exception ex)
            {
                _exceptionProcessor.UpdateResultsWithException(_result.UpdateScenarioResult, ex);
                _scenarioInitializationException = ex;
            }
            finally
            {
                watch.Stop();

                _result.UpdateResult(
                    _preparedSteps.Select(s => s.Result).ToArray(),
                    watch.GetTime());

                _progressNotifier.NotifyScenarioFinished(Result);
            }
            ProcessExceptions();
        }

        private void ProcessExceptions()
        {
            var exception = CollectException();
            if (exception == null)
                return;

            _result.ExecutionException = exception;

            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        private Exception CollectException()
        {
            if (_scenarioInitializationException != null)
                return _scenarioInitializationException;

            if (_result.Status < ExecutionStatus.Ignored)
                return null;

            var exceptions = _result.GetSteps()
                .Where(s => s.Status == _result.Status)
                .Select(s => s.ExecutionException)
                .Where(x => x != null).ToArray();

            return (_result.Status == ExecutionStatus.Ignored || exceptions.Length == 1)
                ? exceptions.First()
                : new AggregateException(exceptions);
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

        [DebuggerStepThrough]
        public void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn)
        {
            _shouldAbortSubStepExecutionFn = shouldAbortExecutionFn ?? throw new ArgumentNullException(nameof(shouldAbortExecutionFn));
        }

        public IScenarioInfo Info => _info;
    }
}