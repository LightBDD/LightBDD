using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Results.Implementation
{
	internal class ScenarioResult : IScenarioResult
	{
		public ScenarioResult(string scenarioName, IEnumerable<IStepResult> steps, string label)
		{
			Name = scenarioName;
			Steps = steps.ToArray();
			Label = label;

			SetStatus();
		}

		private void SetStatus()
		{
			var stepsResult = Steps.OrderByDescending(s => s.Status).FirstOrDefault();
			if (stepsResult == null)
				return;
			Status = stepsResult.Status;
			StatusDetails = stepsResult.StatusDetails;
		}

		#region IScenarioResult Members

		public string Name { get; private set; }
		public ResultStatus Status { get; private set; }
		public IEnumerable<IStepResult> Steps { get; private set; }
		public string StatusDetails { get; private set; }
		public string Label { get; private set; }

		#endregion
	}
}