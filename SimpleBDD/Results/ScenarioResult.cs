using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Scenario result containing name, status and list of steps.
	/// </summary>
	[Serializable]
	public class ScenarioResult
	{
		private readonly IList<StepResult> _steps = new List<StepResult>();

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
			_steps = steps.ToArray();
			Status = _steps.Select(s => s.Status).OrderByDescending(s => s).FirstOrDefault();
		}

		/// <summary>
		/// Scenario name.
		/// </summary>
		public string ScenarioName { get; set; }

		/// <summary>
		/// Scenario status.
		/// </summary>
		public ResultStatus Status { get; set; }

		/// <summary>
		/// Scenario steps.
		/// </summary>
		public IEnumerable<StepResult> Steps
		{
			get { return _steps; }
		}
	}
}