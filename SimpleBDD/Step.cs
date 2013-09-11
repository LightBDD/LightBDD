using System;
using SimpleBDD.Results;

namespace SimpleBDD
{
	internal class Step
	{
		private readonly Action _action;

		public Step(Action action, int stepNumber, int totalStepsCount)
		{
			_action = action;
			Result = new StepResult(stepNumber, totalStepsCount, TextFormatter.Format(action.Method.Name), ResultStatus.NotRun);
		}

		public void Invoke()
		{
			try
			{
				_action();
				Result.Status = ResultStatus.Passed;
			}
			catch (Exception)
			{
				Result.Status = ResultStatus.Failed;
				throw;
			}
		}

		public StepResult Result { get; private set; }
	}
}