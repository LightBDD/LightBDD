using System.Collections.Generic;
using System.Linq;

namespace SimpleBDD.Results.Implementation
{
	internal class ScenarioResult : IScenarioResult
	{
		public ScenarioResult(string scenarioName, IEnumerable<StepResult> steps)
		{
			Name = scenarioName;
			Steps = steps.ToArray();
			Status = Steps.Select(s => s.Status).OrderByDescending(s => s).FirstOrDefault();
		}

		#region IScenarioResult Members

		public string Name { get; set; }
		public ResultStatus Status { get; set; }
		public IEnumerable<IStepResult> Steps { get; private set; }

		#endregion
	}
}