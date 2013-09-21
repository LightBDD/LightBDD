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
		private readonly StepResult _result;
		public IStepResult Result { get { return _result; } }

		public Step(Action action, int stepNumber)
		{
			_action = action;
			_result = new StepResult(stepNumber, NameFormatter.Format(action.Method.Name), ResultStatus.NotRun);
		}

		public void Invoke()
		{
			try
			{
				_action();
				_result.SetStatus(ResultStatus.Passed);
			}
			catch (IgnoreException e)
			{
				_result.SetStatus(ResultStatus.Ignored, e.Message);
				throw;
			}
			catch (InconclusiveException e)
			{
				_result.SetStatus(ResultStatus.Ignored, e.Message);
				throw;
			}
			catch (Exception e)
			{
				_result.SetStatus(ResultStatus.Failed, e.Message);
				throw;
			}
		}
	}
}