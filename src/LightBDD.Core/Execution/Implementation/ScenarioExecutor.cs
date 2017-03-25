using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioExecutor
    {
        private readonly ExtendableExecutor _extendableExecutor;
        [DebuggerStepThrough]
        public ScenarioExecutor(ExtendableExecutor extendableExecutor)
        {
            _extendableExecutor = extendableExecutor;
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(ScenarioInfo scenario, Func<ExtendableExecutor, object, RunnableStep[]> stepsProvider, Func<object> contextProvider, IScenarioProgressNotifier progressNotifier)
        {
            var runnableScenario = new RunnableScenario(scenario, stepsProvider, contextProvider, progressNotifier, _extendableExecutor);
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