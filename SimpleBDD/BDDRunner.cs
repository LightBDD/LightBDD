using System;

namespace SimpleBDD
{
	public class BDDRunner
	{
		public void RunScenario(params Action[] steps)
		{
			int i = 0;
			foreach (Action step in steps)
				PerformStep(step, ++i, steps.Length);
		}

		private void PerformStep(Action step, int stepNumber, int totalStepCount)
		{
			Console.WriteLine("STEP {0}/{1}: {2}", stepNumber, totalStepCount, TextFormatter.Format(step.Method.Name));
			step();
		}
	}
}
