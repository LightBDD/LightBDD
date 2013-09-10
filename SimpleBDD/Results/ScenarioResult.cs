using System.Collections.Generic;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Scenario result containing name, status and list of steps.
	/// </summary>
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
		/// <param name="scenarioName"></param>
		public ScenarioResult(string scenarioName)
		{
			ScenarioName = scenarioName;
		}

		/// <summary>
		/// Scenario name.
		/// </summary>
		public string ScenarioName { get; private set; }

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

		/// <summary>
		/// Adds new step to scenario.
		/// </summary>
		/// <param name="step">Step to add.</param>
		public void AddStep(StepResult step)
		{
			_steps.Add(step);
		}
	}
}