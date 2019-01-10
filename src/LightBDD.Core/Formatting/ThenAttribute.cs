using System;

namespace LightBDD.Core.Formatting
{
	/// <summary>
	/// Then-Step method-attribute
	/// </summary>
	public class ThenAttribute : StepNameAttribute
	{
		/// <summary>
		/// Defaultname for report
		/// </summary>
		public static string DefaultName = "THEN";

		/// <summary>
		/// constructor	
		/// </summary>
		/// <param name="description"></param>
		public ThenAttribute(string description = null) : base(DefaultName, description)
		{
		}
	}
}
