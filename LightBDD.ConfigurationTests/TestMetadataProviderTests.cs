using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace LightBDD.ConfigurationTests
{
    [TestFixture]
    public class TestMetadataProviderTests
    {
        public class TestableTestMetadataProvider : TestMetadataProvider
        {
            protected override string GetImplementationSpecificFeatureDescription(Type testClass)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void Should_initialize_object_with_values_from_app_config()
        {
            var subject = new TestableTestMetadataProvider();
            Assert.That(subject.RepeatedStepReplacement, Is.EqualTo("and also"));
            Assert.That(subject.PredefinedStepTypes, Is.EquivalentTo(new[] { "call", "invoke", "exec" }));
        }
    }
}
