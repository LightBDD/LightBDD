using System;
using LightBDD.Naming;
using LightBDD.Results;
using LightBDD.Results.Implementation;
using NUnit.Framework;

namespace LightBDD
{
	internal class Step
	{
		private readonly Action _action;
		public StepResult Result { get; private set; }

		public Step(Action action, int stepNumber)
		{
			_action = action;
			Result = new StepResult(stepNumber, NameFormatter.Format(action.Method.Name), ResultStatus.NotRun);
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