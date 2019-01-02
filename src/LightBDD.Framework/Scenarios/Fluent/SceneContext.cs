using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class SceneContext<TGiven,TWhen,TThen,TFunc_Given>
	{
		TGiven givenContext;
		TWhen whenContext;
		TThen thenContext;

		static SceneContext<TGiven, TWhen, TThen, TFunc_Given> Instance = null;
		readonly Func<TFunc_Given, IGiven<TFunc_Given>> givenInit;

		public SceneContext(TGiven givenContext, TWhen whenContext, TThen thenContext, Func<TFunc_Given, IGiven<TFunc_Given>> createGiven)
		{
			this.givenInit = createGiven;
			Init(givenContext, whenContext, thenContext);

			Instance = this;
		}

		protected void Init(TGiven givenContext, TWhen whenContext, TThen thenContext)
		{
			this.thenContext = thenContext;
			this.whenContext = whenContext;
			this.givenContext = givenContext;
		}

		IGiven<TFunc_Given> given = null;
		IWhen<TFunc_Given> when = null;
		IThen<TFunc_Given> then
		{
			get => ThenResult<TThen, TFunc_Given>.then;
			set => ThenResult<TThen, TFunc_Given>.then = value;
		}

		public static GivenResult<TGiven, TWhen> CreateGiven(TFunc_Given given)
		{
			if (Instance.given != null)
				Instance.given = Instance.given.And(given);
			else
				Instance.given = Instance.givenInit(given);

			return new GivenResult<TGiven, TWhen>(Instance.givenContext, Instance.whenContext);
		}
		public static WhenResult<TWhen, TThen> CreateWhen(TFunc_Given when)
		{
			if (Instance.when == null)
				Instance.when = Instance.given.When(when);
			else
				Instance.when = Instance.when.And(when);

			return new WhenResult<TWhen, TThen>(Instance.whenContext, Instance.thenContext);
		}
		public static ThenResult<TThen,TFunc_Given> CreateThen(TFunc_Given then)
		{
			if (Instance.then == null)
				Instance.then = Instance.when.Then(then);
			else
				Instance.then = Instance.then.And(then);

			return new ThenResult<TThen,TFunc_Given>(Instance.thenContext);
		}

		public TGiven Given
		{
			get
			{
				return this.givenContext;
			}
		}
	}
}
