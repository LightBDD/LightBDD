using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class SceneBase<T> : IGiven<T>, IWhen<T>, IThen<T>
	{
		protected List<T> actions = new List<T>();
		public T[] End()
		{
			return actions.ToArray();
		}

		protected SceneBase(T given)
		{
			this.actions.Add(given);
		}

		public IGiven<T> And(T given)
		{
			this.actions.Add(given);
			return this;
		}

		public IThen<T> Then(T then)
		{
			this.actions.Add(then);
			return this;
		}

		public IWhen<T> When(T when)
		{
			this.actions.Add(when);
			return this;
		}

		IWhen<T> IWhen<T>.And(T when)
		{
			this.actions.Add(when);
			return this;
		}

		IThen<T> IThen<T>.And(T then)
		{
			this.actions.Add(then);
			return this;
		}
	}
}
