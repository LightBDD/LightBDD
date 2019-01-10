using System;

namespace LightBDD.Core.Formatting
{
	/// <summary>
	/// When-Step method-attribute
	/// </summary>
	public class WhenAttribute : StepNameAttribute
	{
		/// <summary>
		/// Defaultname for report
		/// </summary>
		public static string DefaultName = "WHEN";

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="description"></param>
		public WhenAttribute(string description = null) : base(DefaultName, description)
		{

		}
	}
}
