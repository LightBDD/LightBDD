using System;
using System.Collections.Generic;
using LightBDD.Results;

namespace LightBDD.Execution
{
    internal interface IScenarioExecutor
    {
        void Execute(string scenarioName, string label, IEnumerable<IStep> steps);
        event Action<IScenarioResult> ScenarioExecuted;
    }
}