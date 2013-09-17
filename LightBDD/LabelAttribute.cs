using System;

namespace LightBDD
{
	/// <summary>
	/// Label attribute that can be applied on feature test class or scenario method.
	/// May be used to link feature/scenario with external tools by storing ticket number.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class LabelAttribute : Attribute
	{
		/// <summary>
		/// Specified label.
		/// </summary>
		public string Label { get; private set; }

		/// <summary>
		/// Constructor allowing to associate label text.
		/// </summary>
		/// <param name="label">Label.</param>
		public LabelAttribute(string label)
		{
			Label = label;
		}
	}
}