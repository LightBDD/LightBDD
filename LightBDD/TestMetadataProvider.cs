using System;
using System.Linq;
using System.Reflection;
using LightBDD.Naming;

namespace LightBDD
{
	/// <summary>
	/// Test metadata provider allows to retrieve scenario and feature metadata such as descriptions, labels or names.
	/// </summary>
	public abstract class TestMetadataProvider
	{
		/// <summary>
		/// Retrieves specified attribute property value.
		/// </summary>
		protected static string ExtractAttributePropertyValue<TAttribute>(MemberInfo member, Func<TAttribute, string> valueExtractor) where TAttribute : Attribute
		{
			return member.GetCustomAttributes(typeof(TAttribute), true)
				.OfType<TAttribute>()
				.Select(valueExtractor)
				.SingleOrDefault();
		}

		/// <summary>
		/// Retrieves feature description from [FeatureDescription] attribute.
		/// If attribute is not defined for feature test class, implementation specific feature description is returned.
		/// </summary>
		/// <returns>Feature description string or null if no description is defined.</returns>
		public string GetFeatureDescription(Type featureTestClass)
		{
			return ExtractAttributePropertyValue<FeatureDescriptionAttribute>(featureTestClass, a => a.Description)
				?? GetImplementationSpecificFeatureDescription(featureTestClass);
		}

		/// <summary>
		/// Retrieves feature label from [Label] attribute or null if attribute is not defined.
		/// </summary>
		/// <returns>Feature label string or null if label is not defined.</returns>
		public string GetFeatureLabel(Type featureTestClass)
		{
			return ExtractAttributePropertyValue<LabelAttribute>(featureTestClass, a => a.Label);
		}

		/// <summary>
		/// Retrieves feature name which bases on name of feature test class.
		/// </summary>
		/// <returns>Feature name.</returns>
		public string GetFeatureName(Type featureTestClass)
		{
			return NameFormatter.Format(featureTestClass.Name);
		}

		/// <summary>
		/// Retrieves scenario label from [Label] attribute or null if attribute is not defined.
		/// </summary>
		/// <returns>Scenario label string or null if label is not defined.</returns>
		public string GetScenarioLabel(MethodBase scenarioMethod)
		{
			return ExtractAttributePropertyValue<LabelAttribute>(scenarioMethod, a => a.Label);
		}

		/// <summary>
		/// Retrieves scenario name which bases on name of scenario method.
		/// </summary>
		/// <returns>Scenario name.</returns>
		public string GetScenarioName(MethodBase scenarioMethod)
		{
			return NameFormatter.Format(scenarioMethod.Name);
		}

		/// <summary>
		/// Returns step name which bases on name of scenario step method.
		/// </summary>
		/// <returns>Step name.</returns>
		public string GetStepName(MethodBase stepMethod)
		{
			return NameFormatter.Format(stepMethod.Name);
		}

		/// <summary>
		/// Returns implementation specific feature description or null if such is not provided.
		/// </summary>
		/// <param name="testClass">Class to analyze.</param>
		/// <returns>Feature description or null.</returns>
		protected abstract string GetImplementationSpecificFeatureDescription(Type testClass);
	}
}