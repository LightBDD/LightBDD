using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Core.Formatting
{
	/// <summary>
	/// Attribute to describe the Stepname
	/// </summary>
	public class StepNameAttribute : Attribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Stepname</param>
		/// <param name="description">Text to describe the step</param>
		public StepNameAttribute(string name, string description)
		{
			Name = name;
			Description = description;
		}

		/// <summary>
		/// Name of the step
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Description of the step
		/// </summary>
		public string Description { get; }
	}
}
