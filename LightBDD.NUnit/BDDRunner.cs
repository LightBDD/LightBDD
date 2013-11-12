using System;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD
{
	/// <summary>
	/// Allows to execute behavior test scenarios written with NUnit Framework.
	/// </summary>
	public class BDDRunner : AbstractBDDRunner
	{
		/// <summary>
		/// Initializes runner for given test class type with ConsoleProgressNotifier.
		/// Given testClass type Name is used as feature name.
		/// If test class is annotated with [FeatureDescription] or [Description] attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="testClass">Test class type.</param>
		public BDDRunner(Type testClass)
			: base(testClass)
		{
		}

		/// <summary>
		/// Initializes runner for given test class type with given progress notifier.
		/// Given testClass type Name is used as feature name.
		/// If test class is annotated with [FeatureDescription] or [Description] attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="testClass">Test class type.</param>
		/// <param name="progressNotifier">Progress notifier.</param>
		public BDDRunner(Type testClass, IProgressNotifier progressNotifier)
			: base(testClass, progressNotifier)
		{
		}

		/// <summary>
		/// Returns implementation specific feature description or null if such is not provided.
		/// If test class is annotated with [Description] attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="testClass">Class to analyze.</param>
		/// <returns>Feature description or null.</returns>
		protected override string GetImplementationSpecificFeatureDescription(Type testClass)
		{
			return testClass.GetCustomAttributes(typeof(DescriptionAttribute), true)
				.OfType<DescriptionAttribute>()
				.Select(a => a.Description)
				.SingleOrDefault();
		}

		/// <summary>
		/// Maps implementation specific exception to ResultStatus.
		/// </summary>
		protected override ResultStatus MapExceptionToStatus(Type exceptionType)
		{
			return (typeof(IgnoreException).IsAssignableFrom(exceptionType)
				|| typeof(InconclusiveException).IsAssignableFrom(exceptionType))
				? ResultStatus.Ignored
				: ResultStatus.Failed;
		}
	}
}