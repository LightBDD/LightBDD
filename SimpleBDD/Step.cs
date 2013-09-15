using System;
using NUnit.Framework;
using SimpleBDD.Results;
using SimpleBDD.Results.Implementation;

namespace SimpleBDD
{
	internal class Step
	{
		private readonly Action _action;
		public StepResult Result { get; private set; }

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
			catch (IgnoreException)
			{
				Result.Status = ResultStatus.Ignored;
				throw;
			}
			catch (InconclusiveException)
			{
				Result.Status = ResultStatus.Ignored;
				throw;
			}
			catch (Exception)
			{
				Result.Status = ResultStatus.Failed;
				throw;
			}
		}
	}
}