﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBDD.Framework.Fluent
{
	public class Scene<T> : IGiven<T>, IWhen<T>, IThen<T>
	{
		public static IGiven<Action> Given(Action given)
		{
			return new Scene<Action>(given);
		}

		public static IGiven<Func<Task>> GivenAsync(Func<Task> given)
		{
			return new Scene<Func<Task>>(given);
		}

		protected List<T> actions = new List<T>();
		public T[] End()
		{
			return actions.ToArray();
		}

		Scene(T given)
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
