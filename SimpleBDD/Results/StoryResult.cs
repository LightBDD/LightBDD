using System;
using System.Collections.Generic;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Story results containing list of scenarios.
	/// </summary>
	[Serializable]
	public class StoryResult
	{
		private readonly List<ScenarioResult> _scenarios = new List<ScenarioResult>();

		/// <summary>
		/// List of scenarios for this story.
		/// </summary>
		public IEnumerable<ScenarioResult> Scenarios
		{
			get { return _scenarios; }
		}

		/// <summary>
		/// Adds scenario result.
		/// </summary>
		/// <param name="scenarioResult">Scenario to add.</param>
		public void AddScenario(ScenarioResult scenarioResult)
		{
			_scenarios.Add(scenarioResult);
		}
	}
}