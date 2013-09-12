using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Scenario result containing name, status and list of steps.
	/// </summary>
	[Serializable]
	public class ScenarioResult
	{
		/// <summary>
		/// Scenario name.
		/// </summary>
		[XmlAttribute(AttributeName = "Name")]
		public string ScenarioName { get; set; }

		/// <summary>
		/// Scenario status.
		/// </summary>
		[XmlAttribute(AttributeName = "Status")]
		public ResultStatus Status { get; set; }

		/// <summary>
		/// Scenario steps.
		/// </summary>
		[XmlElement(ElementName = "Steps")]
		public StepResult[] Steps { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ScenarioResult()
		{
		}

		/// <summary>
		/// Initializes scenario result with name.
		/// </summary>
		/// <param name="scenarioName">Scenario name.</param>
		/// <param name="steps">Steps results.</param>
		public ScenarioResult(string scenarioName, IEnumerable<StepResult> steps)
		{
			ScenarioName = scenarioName;
			Steps = steps.ToArray();
			Status = Steps.Select(s => s.Status).OrderByDescending(s => s).FirstOrDefault();
		}
	}
}