using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioExecutor
    {
        private readonly IExtendableExecutor _extendableExecutor;

        public ScenarioExecutor(IExtendableExecutor extendableExecutor)
        {
            _extendableExecutor = extendableExecutor;
        }

        public Task Execute(ScenarioInfo scenario, Func<RunnableStep[]> stepsProvider, Func<object> contextProvider, IScenarioProgressNotifier progressNotifier)
        {
            return _extendableExecutor.ExecuteScenario(scenario,() => ExecuteWithinSynchronizationContext(scenario, stepsProvider, contextProvider, progressNotifier));
        }

        private async Task ExecuteWithinSynchronizationContext(ScenarioInfo scenario, Func<RunnableStep[]> stepsProvider, Func<object> contextProvider, IScenarioProgressNotifier progressNotifier)
        {
            progressNotifier.NotifyScenarioStart(scenario);
            var data = new ScenarioData();
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                data.InitializeScenario(stepsProvider, contextProvider);
                foreach (var step in data.PreparedSteps)
                    await step.Invoke(_extendableExecutor,data.ScenarioContext);
            }
            finally
            {
                watch.Stop();

                var result = new ScenarioResult(
                    scenario,
                    data.PreparedSteps.Select(s => s.Result).ToArray(),
                    watch.GetTime(),
                    data.ScenarioInitializationException);

                progressNotifier.NotifyScenarioFinished(result);
                ScenarioExecuted?.Invoke(result);
            }
        }

        public event Action<IScenarioResult> ScenarioExecuted;
    }
}