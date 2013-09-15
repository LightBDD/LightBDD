using System.Collections.Generic;

namespace SimpleBDD.Results.Implementation
{
	internal class FeatureResult : IFeatureResult
	{
		private readonly List<IScenarioResult> _scenarios = new List<IScenarioResult>();

		public FeatureResult(string name, string description)
		{
			Description = description;
			Name = name;
		}

		public IEnumerable<IScenarioResult> Scenarios { get { return _scenarios; } }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public void AddScenario(IScenarioResult scenario)
		{
			_scenarios.Add(scenario);
		}
	}
}