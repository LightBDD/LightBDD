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
	/// <typeparam name="TWhen">Type of object that is used for WHEN-methods</typeparam>
	/// <typeparam name="TThen">Type of object that is used for THEN-methods</typeparam>
	public class WhenResult<TWhen,TThen>
	{
		/// <summary>
		/// Construtctor
		/// </summary>
		/// <param name="when">object that has the when methods</param>
		/// <param name="then">object that has the then methods</param>
		public WhenResult(TWhen when, TThen then)
		{
			this.Then = then;
			this.And = when;
		}

		/// <summary>
		/// And-link
		/// </summary>
		public TWhen And
		{
			get;
		}
		/// <summary>
		/// Then-link
		/// </summary>
		public TThen Then
		{
			get;
		}
	}
}
