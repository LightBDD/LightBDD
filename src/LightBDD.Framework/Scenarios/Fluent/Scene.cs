using System;
using System.Collections.Generic;

namespace LightBDD.Framework.Scenarios.Fluent
{
	/// <summary>
	/// Implementation of the fluent design for Given-When-Then definition.
	/// By design you have to start with 'Given'-method, then came 'When' and 'Then'.
	/// </summary>
	/// <typeparam name="TAction"></typeparam>
	public class Scene<TAction> : IGiven<TAction>, IWhen<TAction>, IThen<TAction>
	{
		/// <summary>
		/// Runner for the test definition
		/// </summary>
		public IBddRunner Runner
		{
			get;
			private set;
		}
		List<List<TAction>> Givens { get; } = new List<List<TAction>>();
		List<List<TAction>> Whens { get; } = new List<List<TAction>>();
		List<List<TAction>> Thens { get; } = new List<List<TAction>>();

		/// <summary>
		/// End of the definition
		/// </summary>
		/// <returns>Array of steps</returns>
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

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="runner">runner for the test definition</param>
		public Scene(IBddRunner runner)
		{
			this.Runner = runner;
		}

		/// <summary>
		/// Given implementation
		/// </summary>
		/// <param name="given">Action or Func that has to be executed</param>
		/// <returns>Given object</returns>
		public IGiven<TAction> Given(TAction given)
		{
			AddToList(this.Givens, given, newList: true);

			return this;
		}

		/// <summary>
		/// And-Given implementation
		/// </summary>
		/// <param name="given">Action or Func that has to be executed</param>
		/// <returns>given object</returns>
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

		/// <summary>
		/// When implementation
		/// </summary>
		/// <param name="when">Action or Func that has to be executed</param>
		/// <returns>when-object</returns>
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

		/// <summary>
		/// Then implementation
		/// </summary>
		/// <param name="then">Action or Func that has to be executed</param>
		/// <returns>Then-object</returns>
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
