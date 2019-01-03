using System;

namespace LightBDD.Core.Formatting
{
	/// <summary>
	/// Method Attribute for Given-Step
	/// </summary>
	public class GivenAttribute : StepNameAttribute
	{
		/// <summary>
		/// cunstructor
		/// </summary>
		/// <param name="description">Description</param>
		public GivenAttribute(string description = null) : base("GIVEN", description)
		{

		}
	}
}
