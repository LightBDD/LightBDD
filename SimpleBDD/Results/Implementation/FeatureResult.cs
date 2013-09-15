using System.Collections.Generic;

namespace SimpleBDD.Results.Implementation
{
	internal class FeatureResult : IFeatureResult
	{
		private readonly List<IScenarioResult> _scenarios = new List<IScenarioResult>();

		public IEnumerable<IScenarioResult> Scenarios { get { return _scenarios; } }

		public void AddScenario(IScenarioResult scenario)
		{
			_scenarios.Add(scenario);
		}
	}
}