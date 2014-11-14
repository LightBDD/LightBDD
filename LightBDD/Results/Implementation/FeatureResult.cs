using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LightBDD.Results.Implementation
{
    internal class FeatureResult : IFeatureResult
    {
        private readonly List<IScenarioResult> _scenarios = new List<IScenarioResult>();

        public FeatureResult(string name, string description, string label)
        {
            Description = description;
            Name = name;
            Label = label;
        }

        #region IFeatureResult Members

        public IEnumerable<IScenarioResult> Scenarios { get { return GetScenarios(); } }
        public string Label { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        #endregion

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddScenario(IScenarioResult scenario)
        {
            _scenarios.Add(scenario);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private IEnumerable<IScenarioResult> GetScenarios()
        {
            return _scenarios.ToArray();
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Label) 
            ? Name 
            : string.Format("{0} [{1}]", Name, Label);
        }
    }
}