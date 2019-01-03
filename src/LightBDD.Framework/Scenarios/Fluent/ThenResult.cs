using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{

	/// <summary>
	/// If you use context related FluentAPI
	/// <see cref="SceneContext{TGiven, TWhen, TThen, TFunc_Given}"/>
	/// a GIVEN-method has to return an object of this class
	/// </summary>
	/// <typeparam name="TThen">Type of object that is used for THEN-methods</typeparam>
	/// <typeparam name="TAction">Actioon or Func that has to be executed</typeparam>
	public class ThenResult<TThen,TAction>
	{
		internal static IThen<TAction> then;

		/// <summary>
		/// Construtctor
		/// </summary>
		/// <param name="then">object that has the then methods</param>
		public ThenResult(TThen then)
		{
			this.And = then;
		}

		/// <summary>
		/// And-link
		/// </summary>
		public TThen And
		{
			get;
		}

		/// <summary>
		/// Prepare the Run
		/// </summary>
		/// <returns>Then-object</returns>
		public IThen<TAction> To() => then;
	}
}
