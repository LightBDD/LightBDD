using System;
using LightBDD.Notification;
using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    class TestableBDDRunner : AbstractBDDRunner
    {
        public TestableBDDRunner(Type featureTestClass)
            : base(featureTestClass, new TestableMetadataProvider()) { }
        public TestableBDDRunner(Type featureTestClass, IProgressNotifier progressNotifier)
            : base(featureTestClass, new TestableMetadataProvider(), progressNotifier) { }
        public TestableBDDRunner(Type featureTestClass, TestMetadataProvider testableMetadataProvider, IProgressNotifier progressNotifier)
            : base(featureTestClass, testableMetadataProvider, progressNotifier) { }

        protected override ResultStatus MapExceptionToStatus(Type exceptionType)
        {
            return exceptionType == typeof(IgnoreException) ? ResultStatus.Ignored : ResultStatus.Failed;
        }
    }
}