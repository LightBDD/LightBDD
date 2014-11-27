using System;
using System.Collections.Generic;
using LightBDD.Results;

namespace LightBDD.Execution.Implementation
{
    internal interface IScenarioExecutor
    {
        void Execute(Scenario scenario, IEnumerable<IStep> steps);
        event Action<IScenarioResult> ScenarioExecuted;
    }
}