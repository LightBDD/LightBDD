using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution.Implementation;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioExecutor
    {
        private readonly IExtendableExecutor _extendableExecutor;
        [DebuggerStepThrough]
        public ScenarioExecutor(IExtendableExecutor extendableExecutor)
        {
            _extendableExecutor = extendableExecutor;
        }
        [DebuggerStepThrough]
        public Task Execute(ScenarioInfo scenario, Func<RunnableStep[]> stepsProvider, Func<object> contextProvider, IScenarioProgressNotifier progressNotifier)
        {
            return _extendableExecutor.ExecuteScenario(scenario, () => ExecuteWithinSynchronizationContext(scenario, stepsProvider, contextProvider, progressNotifier));
        }
        [DebuggerStepThrough]
        private async Task ExecuteWithinSynchronizationContext(ScenarioInfo scenario, Func<RunnableStep[]> stepsProvider, Func<object> contextProvider, IScenarioProgressNotifier progressNotifier)
        {
            progressNotifier.NotifyScenarioStart(scenario);
            var data = new ScenarioData();
            var watch = ExecutionTimeWatch.StartNew();
            try
            {
                data.InitializeScenario(stepsProvider, contextProvider);
                await ExecuteSteps(data);
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

        private async Task ExecuteSteps(ScenarioData data)
        {
            foreach (var step in data.PreparedSteps)
                await step.Invoke(_extendableExecutor, data.ScenarioContext);
        }

        public event Action<IScenarioResult> ScenarioExecuted;
    }
}