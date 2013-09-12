using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Story results containing list of scenarios.
	/// </summary>
	[Serializable]
	public class StoryResult
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public StoryResult()
		{
			Scenarios = new List<ScenarioResult>();
		}

		/// <summary>
		/// Scenarios.
		/// </summary>
		[XmlElement(ElementName = "Scenarios")]
		public List<ScenarioResult> Scenarios { get; set; }

		/// <summary>
		/// Adds scenario result.
		/// </summary>
		/// <param name="scenarioResult">Scenario to add.</param>
		public void AddScenario(ScenarioResult scenarioResult)
		{
			Scenarios.Add(scenarioResult);
		}
	}
}