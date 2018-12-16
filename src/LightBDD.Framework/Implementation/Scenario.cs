using System;
using System.Collections.Generic;
using System.Text;
using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Basic;

namespace LightBDD.Framework.Implementation
{
	// Given.A.And.B.And.C.When.D.AND.E.Then.F.And.G
	// new Given(a).And(B).And(C).When(D).And(E).Then(F).And(G)

	public interface IGiven
	{
		IGiven And(Action given);
		IWhen When(Action when);
	}

	public interface IWhen
	{
		IWhen And(Action when);
		IThen Then(Action then);
	}

	public interface IThen {
		IThen And(Action then);
		void Run(IBddRunner runner);
	}

	public class Scenario : IGiven,IWhen,IThen
	{
		private readonly List<Action> actions = new List<Action>();

		public static IGiven Given(Action given) {
			return new Scenario(given);
		}

		Scenario(Action given)
		{
			this.actions.Add(given);
		}

		public void Run(IBddRunner runner)
		{
			runner.RunScenario(this.actions.ToArray());
		}

		public IGiven And(Action given)
		{
			this.actions.Add(given);
			return this;
		}

		IWhen IWhen.And(Action when)
		{
			this.actions.Add(when);
			return this;
		}

		IThen IThen.And(Action then)
		{
			this.actions.Add(then);
			return this;
		}

		public IWhen When(Action when)
		{
			this.actions.Add(when);
			return this;
		}

		public IThen Then(Action then)
		{
			this.actions.Add(then);
			return this;
		}
	}
}
