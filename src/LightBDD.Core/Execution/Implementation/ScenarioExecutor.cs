using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioExecutor
    {
        private readonly DecoratingExecutor _decoratingExecutor;
        [DebuggerStepThrough]
        public ScenarioExecutor(DecoratingExecutor decoratingExecutor)
        {
            _decoratingExecutor = decoratingExecutor;
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(ScenarioInfo scenario, Func<DecoratingExecutor, object, RunnableStep[]> stepsProvider, IContextProvider contextProvider, IScenarioProgressNotifier progressNotifier, IEnumerable<IScenarioDecorator> scenarioDecorators, ExceptionProcessor exceptionProcessor)
        {
            var runnableScenario = new RunnableScenario(scenario, stepsProvider, contextProvider, progressNotifier, _decoratingExecutor, scenarioDecorators, exceptionProcessor);
            try
            {
                return runnableScenario.RunAsync();
            }
            finally
            {
                ScenarioExecuted?.Invoke(runnableScenario.Result);
            }
        }

        public event Action<IScenarioResult> ScenarioExecuted;
    }
}