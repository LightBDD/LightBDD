using System;

namespace LightBDD
{
	/// <summary>
	/// Feature description attribute that can be applied on feature test class.
	/// May be used to enrich feature class with description like "In order to... As a... I want to..."
	/// or similar, that would be used by progress notifier and would be included in summary.
	///
	/// If given implementation supports alternative description attributes, and both are applied on class, this one would be used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class FeatureDescriptionAttribute : Attribute
	{
		/// <summary>
		/// Feature description.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Constructor allowing to associate description.
		/// </summary>
		/// <param name="description"></param>
		public FeatureDescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}