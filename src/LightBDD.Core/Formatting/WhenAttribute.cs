using System;

namespace LightBDD.Core.Formatting
{
	/// <summary>
	/// When-Step method-attribute
	/// </summary>
	public class WhenAttribute : StepNameAttribute
	{
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="description"></param>
		public WhenAttribute(string description = null) : base("WHEN", description)
		{

		}
	}
}
