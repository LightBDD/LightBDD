using System;

namespace LightBDD.Core.Formatting
{
	/// <summary>
	/// Then-Step method-attribute
	/// </summary>
	public class ThenAttribute : StepNameAttribute
	{
		/// <summary>
		/// constructor	
		/// </summary>
		/// <param name="description"></param>
		public ThenAttribute(string description = null) : base("THEN", description)
		{
		}
	}
}
