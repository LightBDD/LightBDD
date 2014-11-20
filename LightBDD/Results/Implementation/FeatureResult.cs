using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LightBDD.Results.Implementation
{
    internal class FeatureResult : IFeatureResult
    {
        private readonly List<IScenarioResult> _scenarios = new List<IScenarioResult>();
        private readonly string[] _categories;

        public FeatureResult(string name, string description, string label, IEnumerable<string> categories)
        {
            Description = description;
            Name = name;
            Label = label;
            _categories = categories.ToArray();
        }

        #region IFeatureResult Members

        public IEnumerable<IScenarioResult> Scenarios { get { return GetScenarios(); } }
        public IEnumerable<string> Categories { get { return _categories; } }
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