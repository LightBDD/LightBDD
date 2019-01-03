using System;

namespace LightBDD.Framework.Scenarios.Fluent
{
	/// <summary>
	/// Then interface to define the methods for then-action
	/// </summary>
	/// <typeparam name="T">parameter Type of the methods. Action or Func</typeparam>
	public interface IThen<T>
	{
		/// <summary>
		/// And
		/// </summary>
		/// <param name="when"></param>
		/// <returns></returns>
		IThen<T> And(T when);
		/// <summary>
		/// Then
		/// </summary>
		/// <param name="when"></param>
		/// <returns></returns>
		IThen<T> Then(T when);
		/// <summary>
		/// End of the Given-When-Then definition
		/// </summary>
		/// <returns>Return the steps of the Given-When-Then definition</returns>
		T[] End();		
	}
}
