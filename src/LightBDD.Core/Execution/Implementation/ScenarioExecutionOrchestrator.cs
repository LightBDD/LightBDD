#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly SemaphoreSlim _executionModeSem = new(1);

        public ScenarioExecutionOrchestrator(ExecutionPipelineConfiguration cfg)
        {
            _scenarioThrottler = new(Math.Max(1, cfg.MaxConcurrentScenarios));
        }

        private async Task<IScenarioResult> Throttle(Func<Task<IScenarioResult>> scenarioFn, IScenarioExecutionScheduler scheduler)
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

        public async Task<IFeatureResult[]> Execute(IReadOnlyList<ExecutionPipeline.RunnableFeature> runnableFeatures)
        {
            await Task.WhenAll(runnableFeatures.SelectMany(f => f.GetRunnableCases())
                .OrderByDescending(c => c.Priority)
                .ThenBy(c => c.Scenario.FeatureFixtureType.Name)
                .ThenBy(c => c.Scenario.ScenarioMethod.Name)
                .GroupBy(c => c.RequireExclusiveRun)
                .OrderBy(x => x.Key)
                .Select(ExecuteGroup));

            return runnableFeatures.Select(r => r.Result!).ToArray();
        }

        private async Task ExecuteGroup(IGrouping<bool, ExecutionPipeline.RunnableScenarioCase> group)
        {
            await _executionModeSem.WaitAsync();
            try
            {
                if (group.Key)
                    await ExecuteSerially(group);
                else
                    await Task.WhenAll(group.Select(s => Throttle(s.Execute, s.Scheduler)));
            }
            finally
            {
                _executionModeSem.Release();
            }
        }

        private async Task ExecuteSerially(IEnumerable<ExecutionPipeline.RunnableScenarioCase> scenarios)
        {
            List<Exception>? exceptions = null;
            foreach (var scenario in scenarios)
            {
                try
                {
                    await scenario.Scheduler.Schedule(scenario.Execute);
                }
                catch (Exception ex)
                {
                    (exceptions ??= new()).Add(ex);
                }
            }

            if (exceptions != null)
                throw new AggregateException(exceptions);
        }
    }
}
