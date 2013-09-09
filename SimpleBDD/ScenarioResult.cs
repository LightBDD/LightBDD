using System.Collections.Generic;

namespace SimpleBDD
{
	public class ScenarioResult
	{
		private readonly IList<StepResult> _steps = new List<StepResult>();

		public ScenarioResult(string scenarioName)
		{
			ScenarioName = scenarioName;
		}

		public string ScenarioName { get; private set; }

		public ResultStatus Status { get; set; }

		public IEnumerable<StepResult> Steps
		{
			get { return _steps; }
		}

		public void AddStep(StepResult step)
		{
			_steps.Add(step);
		}
	}
}