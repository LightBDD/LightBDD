using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Extended;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class Scene<T> : IGiven<T>, IWhen<T>, IThen<T>
	{
		readonly List<List<T>> givens = new List<List<T>>();
		readonly List<List<T>> whens = new List<List<T>>();
		readonly List<List<T>> thens = new List<List<T>>();

		public IBddRunner Runner
		{
			get;
			private set;
		}

		public T[] End()
		{
			var list = new List<T>();

			AddTo(list, this.givens);
			AddTo(list, this.whens);
			AddTo(list, this.thens);

			return list.ToArray();
		}

		private void AddTo(List<T> list, List<List<T>> childList)
		{
			foreach (var item in childList)
				list.AddRange(item);
		}

		public Scene(IBddRunner runner)
		{
			this.Runner = runner;
		}

		public IGiven<T> Given(T given)
		{
			AddToList(this.givens, given, newList: true);

			return this;
		}

		public IGiven<T> And(T given)
		{
			AddToList(this.givens, given);

			return this;
		}

		private void AddToList(List<List<T>> list, T add, bool newList = false)
		{
			if (list.Count == 0 || newList)
				list.Add(new List<T>());

			list[list.Count - 1].Add(add);
		}

		public IWhen<T> When(T when)
		{
			AddToList(this.whens, when, newList: true);

			return this;
		}

		IWhen<T> IWhen<T>.And(T when)
		{
			AddToList(this.whens, when);

			return this;
		}

		public IThen<T> Then(T then)
		{
			AddToList(this.thens, then, newList: true);

			return this;
		}
		IThen<T> IThen<T>.And(T then)
		{
			AddToList(this.thens, then);

			return this;
		}
	}
}
