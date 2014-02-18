using System;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests
{
	class TestableBDDRunner : AbstractBDDRunner
	{
		public TestableBDDRunner(Type featureTestClass) : base(featureTestClass, new TestableMetadataProvider()) { }
		public TestableBDDRunner(Type featureTestClass, IProgressNotifier progressNotifier)
			: base(featureTestClass, new TestableMetadataProvider(), progressNotifier)
		{
		}

		protected override ResultStatus MapExceptionToStatus(Type exceptionType)
		{
			return exceptionType == typeof(IgnoreException) ? ResultStatus.Ignored : ResultStatus.Failed;
		}
	}

	internal class TestableMetadataProvider : TestMetadataProvider
	{
		protected override string GetImplementationSpecificFeatureDescription(Type testClass)
		{
			return testClass.GetCustomAttributes(typeof(DescriptionAttribute), true)
				.OfType<DescriptionAttribute>()
				.Select(a => a.Description)
				.SingleOrDefault();
		}
	}
}