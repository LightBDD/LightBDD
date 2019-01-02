using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Extended;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class Scene<TAction> : IGiven<TAction>, IWhen<TAction>, IThen<TAction>
	{
		public IBddRunner Runner
		{
			get;
			private set;
		}
		List<List<TAction>> Givens { get; } = new List<List<TAction>>();
		List<List<TAction>> Whens { get; } = new List<List<TAction>>();
		List<List<TAction>> Thens { get; } = new List<List<TAction>>();

		public TAction[] End()
		{
			var list = new List<TAction>();

			AddTo(list, this.Givens);
			AddTo(list, this.Whens);
			AddTo(list, this.Thens);

			return list.ToArray();
		}

		private void AddTo(List<TAction> list, List<List<TAction>> childList)
		{
			foreach (var item in childList)
				list.AddRange(item);
		}

		public Scene(IBddRunner runner)
		{
			this.Runner = runner;
		}

		public IGiven<TAction> Given(TAction given)
		{
			AddToList(this.Givens, given, newList: true);

			return this;
		}

		public IGiven<TAction> And(TAction given)
		{
			AddToList(this.Givens, given);

			return this;
		}

		private void AddToList(List<List<TAction>> list, TAction add, bool newList = false)
		{
			if (list.Count == 0 || newList)
				list.Add(new List<TAction>());

			list[list.Count - 1].Add(add);
		}

		public IWhen<TAction> When(TAction when)
		{
			AddToList(this.Whens, when, newList: true);

			return this;
		}

		IWhen<TAction> IWhen<TAction>.And(TAction when)
		{
			AddToList(this.Whens, when);

			return this;
		}

		public IThen<TAction> Then(TAction then)
		{
			AddToList(this.Thens, then, newList: true);

			return this;
		}
		IThen<TAction> IThen<TAction>.And(TAction then)
		{
			AddToList(this.Thens, then);

			return this;
		}
	}
}
