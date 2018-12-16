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

	public class Feature : IGiven,IWhen,IThen
	{
		private readonly List<Action> givens = new List<Action>();
		private readonly List<Action> whens = new List<Action>();
		private readonly List<Action> thens = new List<Action>();

		public static IGiven Given(Action given) {
			return new Feature(given);
		}

		Feature(Action given)
		{
			this.givens.Add(given);
		}

		public void Run(IBddRunner runner)
		{
			var list = new List<Action>();
			list.AddRange(this.givens);
			list.AddRange(this.whens);
			list.AddRange(this.thens);

			runner.RunScenario(list.ToArray());
		}

		public IGiven And(Action given)
		{
			this.givens.Add(given);
			return this;
		}

		IWhen IWhen.And(Action when)
		{
			this.whens.Add(when);
			return this;
		}

		IThen IThen.And(Action then)
		{
			this.thens.Add(then);
			return this;
		}

		public IWhen When(Action when)
		{
			this.whens.Add(when);
			return this;
		}

		public IThen Then(Action then)
		{
			this.thens.Add(then);
			return this;
		}
	}
}
