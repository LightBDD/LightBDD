using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;

namespace LightBDD.AcceptanceTests.Helpers.Builders
{
    internal class ResultBuilder
    {
        private readonly List<FeatureBuilder> _features = new List<FeatureBuilder>();

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

        public IFeatureResult[] Build()
        {
            return _features.Select(f => f.Build()).ToArray();
        }
    }
}