using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Implementation
{
    internal class ScenarioExecutor
    {
        public async Task Execute(ScenarioInfo scenario, IEnumerable<RunnableStep> steps)
        {
            var preparedSteps = steps.ToArray();

            var watch = ExecutionTimeWatch.StartNew();

            try
            {
                foreach (var step in preparedSteps)
                    await step.Invoke(null);
            }
            finally
            {
                watch.Stop();

                var result = new ScenarioResult(
                    scenario,
                    preparedSteps.Select(s => s.Result).ToArray(),
                    watch.GetTime());

                ScenarioExecuted?.Invoke(result);
            }
        }

        public event Action<IScenarioResult> ScenarioExecuted;
    }
}