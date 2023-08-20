using System;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioExecutionScheduler
    {
        private readonly SemaphoreSlim _scenarioThrottler;

        public ScenarioExecutionScheduler(ExecutionPipelineConfiguration cfg)
        {
            _scenarioThrottler = new(Math.Max(1, cfg.MaxConcurrentScenarios));
        }

        public async Task<IScenarioResult> Schedule(Func<Task<IScenarioResult>> scenarioFn)
        {
            try
            {
                await _scenarioThrottler.WaitAsync();
                return await scenarioFn();
            }
            finally
            {
                _scenarioThrottler.Release();
            }
        }
    }
}
