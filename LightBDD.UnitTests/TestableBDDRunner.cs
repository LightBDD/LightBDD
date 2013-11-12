using System;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests
{
	class TestableBDDRunner : AbstractBDDRunner
	{
		public TestableBDDRunner(Type testClass) : base(testClass) { }
		public TestableBDDRunner(Type testClass, IProgressNotifier progressNotifier)
			: base(testClass, progressNotifier)
		{
		}

		protected override string GetImplementationSpecificFeatureDescription(Type testClass)
		{
			return testClass.GetCustomAttributes(typeof(DescriptionAttribute), true)
				.OfType<DescriptionAttribute>()
				.Select(a => a.Description)
				.SingleOrDefault();
		}

		protected override ResultStatus MapExceptionToStatus(Type exceptionType)
		{
			return exceptionType == typeof(IgnoreException) ? ResultStatus.Ignored : ResultStatus.Failed;
		}
	}
}