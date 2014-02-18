using System;
using NUnit.Framework;

namespace LightBDD
{
	internal class NUnitTestMetadataProvider : TestMetadataProvider
	{
		public static readonly TestMetadataProvider Instance = new NUnitTestMetadataProvider();

		/// <summary>
		/// Returns implementation specific feature description or null if such is not provided.
		/// If test class is annotated with [Description] attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="testClass">Class to analyze.</param>
		/// <returns>Feature description or null.</returns>
		protected override string GetImplementationSpecificFeatureDescription(Type testClass)
		{
			return ExtractAttributePropertyValue<DescriptionAttribute>(testClass, a => a.Description);
		}
	}
}