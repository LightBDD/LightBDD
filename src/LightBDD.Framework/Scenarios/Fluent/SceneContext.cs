using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class SceneContext<G,W,T,A>
	{
		readonly G givenContext;
		readonly W whenContext;
		readonly T thenContext;

		static SceneContext<G, W, T, A> Instance = null;
		readonly Func<A, IGiven<A>> givenInit;

		public SceneContext(G givenContext, W whenContext, T thenContext, Func<A, IGiven<A>> createGiven)
		{
			this.givenInit = createGiven;
			this.thenContext = thenContext;
			this.whenContext = whenContext;
			this.givenContext = givenContext;

			Instance = this;
		}

		IGiven<A> given = null;
		IWhen<A> when = null;
		IThen<A> then
		{
			get => ThenResult<T, A>.then;
			set => ThenResult<T, A>.then = value;
		}

		public static GivenResult<G, W> CreateGiven(A given)
		{
			if (Instance.given != null)
				Instance.given = Instance.given.And(given);
			else
				Instance.given = Instance.givenInit(given);

			return new GivenResult<G, W>(Instance.givenContext, Instance.whenContext);
		}
		public static WhenResult<W, T> CreateWhen(A when)
		{
			if (Instance.when == null)
				Instance.when = Instance.given.When(when);
			else
				Instance.when = Instance.when.And(when);

			return new WhenResult<W, T>(Instance.whenContext, Instance.thenContext);
		}
		public static ThenResult<T,A> CreateThen(A then)
		{
			if (Instance.then == null)
				Instance.then = Instance.when.Then(then);
			else
				Instance.then = Instance.then.And(then);

			return new ThenResult<T,A>(Instance.thenContext);
		}

		public G Given
		{
			get
			{
				return this.givenContext;
			}
		}
	}
}
