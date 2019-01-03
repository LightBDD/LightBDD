using System;

namespace LightBDD.Framework.Scenarios.Fluent
{
	/// <summary>
	/// Given interface to define the methods for given-action
	/// </summary>
	/// <typeparam name="T">parameter Type of the methods. Action or Func</typeparam>
	public interface IGiven<T>
	{
		/// <summary>
		/// Given-and link
		/// </summary>
		/// <param name="given">method or lamda that will be executing</param>
		/// <returns>Given-object</returns>
		IGiven<T> And(T given);
		/// <summary>
		/// Given
		/// </summary>
		/// <param name="given">method or lamda that will be executing</param>
		/// <returns>Given-object</returns>
		IGiven<T> Given(T given);
		/// <summary>
		/// When
		/// </summary>
		/// <param name="when">method or lamda that will be executing</param>
		/// <returns>when-object</returns>
		IWhen<T> When(T when);
	}
}
