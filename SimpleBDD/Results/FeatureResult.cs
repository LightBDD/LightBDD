using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Story results containing list of scenarios.
	/// </summary>
	[Serializable]
	public class FeatureResult
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public FeatureResult()
		{
			Scenarios = new List<ScenarioResult>();
		}

		/// <summary>
		/// Scenarios.
		/// </summary>
		[XmlElement(ElementName = "Scenario")]
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