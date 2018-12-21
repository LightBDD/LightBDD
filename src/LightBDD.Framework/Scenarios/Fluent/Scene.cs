using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Extended;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class Scene<T> : IGiven<T>, IWhen<T>, IThen<T>
	{
		public IBddRunner Runner
		{
			get;
			private set;
		}
		public List<List<T>> Givens { get; } = new List<List<T>>();
		public List<List<T>> Whens { get; } = new List<List<T>>();
		public List<List<T>> Thens { get; } = new List<List<T>>();

		public T[] End()
		{
			var list = new List<T>();

			AddTo(list, this.Givens);
			AddTo(list, this.Whens);
			AddTo(list, this.Thens);

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
			AddToList(this.Givens, given, newList: true);

			return this;
		}

		public IGiven<T> And(T given)
		{
			AddToList(this.Givens, given);

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
			AddToList(this.Whens, when, newList: true);

			return this;
		}

		IWhen<T> IWhen<T>.And(T when)
		{
			AddToList(this.Whens, when);

			return this;
		}

		public IThen<T> Then(T then)
		{
			AddToList(this.Thens, then, newList: true);

			return this;
		}
		IThen<T> IThen<T>.And(T then)
		{
			AddToList(this.Thens, then);

			return this;
		}
	}
}
