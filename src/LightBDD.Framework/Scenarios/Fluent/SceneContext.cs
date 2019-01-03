using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{
	/// <summary>
	/// fluent design of the Given-When-then definition.
	/// Every part can be collect in class. So you have
	/// a seperate class for givens, whens and thens.
	/// In huge scenarios maybe is easier to share functionality.
	/// </summary>
	/// <typeparam name="TGiven">Type of the class for given-methods</typeparam>
	/// <typeparam name="TWhen">Type of the class for when-methods</typeparam>
	/// <typeparam name="TThen">Type of the class for then-methods</typeparam>
	/// <typeparam name="TFunc_Given">Func to return the first Given-object</typeparam>
	public class SceneContext<TGiven,TWhen,TThen,TFunc_Given>
	{
		TGiven givenContext;
		TWhen whenContext;
		TThen thenContext;

		static SceneContext<TGiven, TWhen, TThen, TFunc_Given> Instance = null;
		readonly Func<TFunc_Given, IGiven<TFunc_Given>> givenInit;

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="givenContext">given-methods object</param>
		/// <param name="whenContext">when-methods object</param>
		/// <param name="thenContext">then-methods object</param>
		/// <param name="createGiven">Func for the first given-object</param>
		public SceneContext(TGiven givenContext, TWhen whenContext, TThen thenContext, Func<TFunc_Given, IGiven<TFunc_Given>> createGiven)
		{
			this.givenInit = createGiven;
			Init(givenContext, whenContext, thenContext);

			Instance = this;
		}

		/// <summary>
		/// Initialization the context objects.
		/// </summary>
		/// <param name="givenContext"></param>
		/// <param name="whenContext"></param>
		/// <param name="thenContext"></param>
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

		/// <summary>
		/// Create a GivenResult. 
		/// Every Given-method has to return such a GivenResult
		/// </summary>
		/// <param name="given">Action or Func that should be executed as test</param>
		/// <returns>GivenResult</returns>
		public static GivenResult<TGiven, TWhen> CreateGiven(TFunc_Given given)
		{
			if (Instance.given != null)
				Instance.given = Instance.given.And(given);
			else
				Instance.given = Instance.givenInit(given);

			return new GivenResult<TGiven, TWhen>(Instance.givenContext, Instance.whenContext);
		}
		/// <summary>
		/// Create a WhenResult. 
		/// Every When-method has to return such a WhenResult
		/// </summary>
		/// <param name="when">Action or Func that should be executed as test</param>
		/// <returns>WhenResult</returns>
		public static WhenResult<TWhen, TThen> CreateWhen(TFunc_Given when)
		{
			if (Instance.when == null)
				Instance.when = Instance.given.When(when);
			else
				Instance.when = Instance.when.And(when);

			return new WhenResult<TWhen, TThen>(Instance.whenContext, Instance.thenContext);
		}

		/// <summary>
		/// Create a ThenResult. 
		/// Every Then-method has to return such a ThenResult
		/// </summary>
		/// <param name="then">Action or Func that should be executed as test</param>
		/// <returns>ThenResult</returns>
		public static ThenResult<TThen,TFunc_Given> CreateThen(TFunc_Given then)
		{
			if (Instance.then == null)
				Instance.then = Instance.when.Then(then);
			else
				Instance.then = Instance.then.And(then);

			return new ThenResult<TThen,TFunc_Given>(Instance.thenContext);
		}

		/// <summary>
		/// First Given call.
		/// </summary>
		public TGiven Given
		{
			get
			{
				return this.givenContext;
			}
		}
	}
}
