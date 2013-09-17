using System.Collections.Generic;

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

		public IEnumerable<IScenarioResult> Scenarios { get { return _scenarios; } }
		public string Label { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		#endregion

		public void AddScenario(IScenarioResult scenario)
		{
			_scenarios.Add(scenario);
		}
	}
}