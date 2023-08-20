using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;
using LightBDD.ScenarioHelpers;

namespace LightBDD.AcceptanceTests.Helpers.Builders
{
    internal class FeatureBuilder
    {
        private readonly List<ScenarioBuilder> _scenarios = new();

        public FeatureBuilder(string feature)
        {
            Name = feature;
        }

        public string Name { get; }

        public ScenarioBuilder NewScenario(ExecutionStatus status)
        {
            var builder = new ScenarioBuilder(status).WithSampleSteps();
            _scenarios.Add(builder);
            return builder;
        }

        public TestResults.TestFeatureResult Build()
        {
            return TestResults.CreateFeatureResult(Name, "descr", "label", _scenarios.Select(s => s.Build()).ToArray());
        }

        public ScenarioBuilder NewEmptyScenario(string scenario, ExecutionStatus status)
        {
            var builder = new ScenarioBuilder(status).WithName(scenario);
            _scenarios.Add(builder);
            return builder;
        }

        public ScenarioBuilder ForScenario(string scenario)
        {
            return _scenarios.First(s => s.Name == scenario);
        }
    }
}