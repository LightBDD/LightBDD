using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Implementation
{
    internal class FeatureResult : IFeatureResult
    {
        private readonly ConcurrentQueue<IScenarioResult> _scenarios = new();

        public FeatureResult(IFeatureInfo info)
        {
            Info = info;
        }

        public IFeatureInfo Info { get; }
        public IEnumerable<IScenarioResult> GetScenarios() { return _scenarios; }
        public void AddScenario(IScenarioResult scenario) { _scenarios.Enqueue(scenario); }

        public override string ToString()
        {
            return Info.ToString();
        }
    }
}