using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Results.Implementation
{
	internal class ScenarioResult : IScenarioResult
	{
		public ScenarioResult(string scenarioName, IEnumerable<StepResult> steps, string label)
		{
			Name = scenarioName;
			Steps = steps.ToArray();
			Label = label;
			Status = Steps.Select(s => s.Status).OrderByDescending(s => s).FirstOrDefault();
		}

		#region IScenarioResult Members

		public string Name { get; set; }
		public ResultStatus Status { get; set; }
		public IEnumerable<IStepResult> Steps { get; private set; }
		public string Label { get; private set; }

		#endregion
	}
}