using System;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Scheduling;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioExecutionOrchestrator
    {
        private readonly SemaphoreSlim _scenarioThrottler;

        public ScenarioExecutionOrchestrator(ExecutionPipelineConfiguration cfg)
        {
            _scenarioThrottler = new(Math.Max(1, cfg.MaxConcurrentScenarios));
        }

        public async Task<IScenarioResult> Execute(Func<Task<IScenarioResult>> scenarioFn, IScenarioExecutionScheduler scheduler)
        {
            try
            {
                await _scenarioThrottler.WaitAsync();
                return await scheduler.Schedule(scenarioFn);
            }
            finally
            {
                _scenarioThrottler.Release();
            }
        }
    }
}
