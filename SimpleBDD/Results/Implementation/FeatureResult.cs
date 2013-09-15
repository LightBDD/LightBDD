using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleBDD.Results.Implementation
{
	/// <summary>
	/// Story results containing list of scenarios.
	/// </summary>
	[Serializable]
	public class FeatureResult : IFeatureResult
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public FeatureResult()
		{
			ScenarioList = new List<ScenarioResult>();
		}

		/// <summary>
		/// Scenarios.
		/// </summary>
		[XmlElement(ElementName = "Scenario")]
		public List<ScenarioResult> ScenarioList { get; set; }

		/// <summary>
		/// Returns executed scenarios for given feature.
		/// </summary>
		public IEnumerable<IScenarioResult> Scenarios { get { return ScenarioList; } }
	}
}