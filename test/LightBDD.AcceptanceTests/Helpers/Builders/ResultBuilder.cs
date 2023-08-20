using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;
using LightBDD.ScenarioHelpers;

namespace LightBDD.AcceptanceTests.Helpers.Builders
{
    internal class ResultBuilder
    {
        private readonly List<FeatureBuilder> _features = new();

        public FeatureBuilder NewFeature(string feature)
        {
            var featureBuilder = new FeatureBuilder(feature);
            _features.Add(featureBuilder);
            return featureBuilder;
        }

        public FeatureBuilder ForFeature(string feature)
        {
            return _features.Single(f => f.Name == feature);
        }

        public ITestRunResult Build()
        {
            return TestResults.CreateTestRunResults(_features.Select(f => f.Build()).ToArray());
        }
    }
}