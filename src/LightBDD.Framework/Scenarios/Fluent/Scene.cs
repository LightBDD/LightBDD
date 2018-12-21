using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Extended;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class Scene<T> : IGiven<T>, IWhen<T>, IThen<T>
	{
		readonly protected List<T> actions = new List<T>();

		public IBddRunner Runner
		{
			get;
			private set;
		}

		public T[] End()
		{
			return actions.ToArray();
		}

		public Scene(IBddRunner runner)
		{
			this.Runner = runner;
		}

		public IGiven<T> Given(T given)
		{
			this.actions.Add(given);
			return this;
		}

		public IGiven<T> And(T given)
		{
			return Given(given);
		}


		public IWhen<T> When(T when)
		{
			this.actions.Add(when);
			return this;
		}

		IWhen<T> IWhen<T>.And(T when)
		{			
			return When(when);
		}

		public IThen<T> Then(T then)
		{
			this.actions.Add(then);
			return this;
		}
		IThen<T> IThen<T>.And(T then)
		{
			return Then(then);
		}
	}
}
