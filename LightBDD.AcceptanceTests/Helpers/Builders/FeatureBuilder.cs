using System.Collections.Generic;
using System.Linq;
using LightBDD.Results;

namespace LightBDD.AcceptanceTests.Helpers.Builders
{
    class FeatureBuilder
    {
        private readonly List<ScenarioBuilder> _scenarios = new List<ScenarioBuilder>();

        public FeatureBuilder(string feature)
        {
            Name = feature;
        }

        public string Name { get; private set; }

        public ScenarioBuilder NewScenario(ResultStatus status)
        {
            var builder = new ScenarioBuilder(status);
            _scenarios.Add(builder);
            return builder;
        }

        public IFeatureResult Build()
        {
            return Mocks.CreateFeatureResult(Name, "descr", "label", _scenarios.Select(s => s.Build()).ToArray());
        }
    }
}