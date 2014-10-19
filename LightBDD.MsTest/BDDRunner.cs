using System;
using LightBDD.Notification;
using LightBDD.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD
{
	/// <summary>
	/// Allows to execute behavior test scenarios written with MbUnit Framework.
	/// </summary>
	public class BDDRunner : AbstractBDDRunner
	{
		/// <summary>
		/// Initializes runner for given test class type with ConsoleProgressNotifier.
		/// Given testClass type Name is used as feature name.
		/// If test class is annotated with [FeatureDescription] or [Description] attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="featureTestClass">Test class type.</param>
		public BDDRunner(Type featureTestClass)
			: base(featureTestClass, MsTestTestMetadataProvider.Instance)
		{
		}

		/// <summary>
		/// Initializes runner for given test class type with given progress notifier.
		/// Given testClass type Name is used as feature name.
		/// If test class is annotated with [FeatureDescription] or [Description] attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="featureTestClass">Test class type.</param>
		/// <param name="progressNotifier">Progress notifier.</param>
		public BDDRunner(Type featureTestClass, IProgressNotifier progressNotifier)
			: base(featureTestClass, MsTestTestMetadataProvider.Instance, progressNotifier)
		{
		}

		/// <summary>
		/// Maps implementation specific exception to ResultStatus.
		/// </summary>
		protected override ResultStatus MapExceptionToStatus(Type exceptionType)
		{
			return (typeof(AssertInconclusiveException).IsAssignableFrom(exceptionType))
				? ResultStatus.Ignored
				: ResultStatus.Failed;
		}
	}
}