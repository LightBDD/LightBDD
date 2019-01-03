using System;

namespace LightBDD.Framework.Scenarios.Fluent
{
	/// <summary>
	/// When interface to define the methods for when-action
	/// </summary>
	/// <typeparam name="T">parameter Type of the methods. Action or Func</typeparam>
	public interface IWhen<T>
	{
		/// <summary>
		/// And
		/// </summary>
		/// <param name="when">When object</param>
		/// <returns></returns>
		IWhen<T> And(T when);
		/// <summary>
		/// When
		/// </summary>
		/// <param name="when">When object</param>
		/// <returns></returns>
		IWhen<T> When(T when);
		/// <summary>
		/// Then
		/// </summary>
		/// <param name="then">then-object</param>
		/// <returns></returns>
		IThen<T> Then(T then);
	}
}
