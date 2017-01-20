using System.Linq;
using System.Reflection;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class CoreMetadataProvider_metadata_inheritance_tests
    {
        private CoreMetadataProvider _metadataProvider;

        [SetUp]
        public void SetUp()
        {
            _metadataProvider = new TestMetadataProvider(new DefaultNameFormatter());
        }

        [Test]
        public void Should_collect_categories_from_method_and_classes()
        {
            Assert.That(
                _metadataProvider.GetScenarioCategories(typeof(DerivedClass).GetMethod("MethodC")).ToArray(),
                Is.EqualTo(new[] { "BaseC", "DerivedC", "DerivedMC" }));
        }

        [Test]
        public void Should_collect_categories_from_method_with_base_and_classes()
        {
            Assert.That(
                _metadataProvider.GetScenarioCategories(typeof(DerivedClass).GetMethod("MethodB")).ToArray(),
                Is.EqualTo(new[] { "BaseC", "BaseMB", "DerivedC", "DerivedMB" }));
        }

        [Test]
        public void Should_collect_categories_from_base_method_and_base_class_only_if_method_is_declared_in_base()
        {
            Assert.That(
                _metadataProvider.GetScenarioCategories(typeof(DerivedClass).GetMethod("MethodA")).ToArray(),
                Is.EqualTo(new[] { "BaseC", "BaseMA" }));
        }

        [Test]
        public void Should_collect_redefined_feature_description_from_derived_class()
        {
            Assert.That(_metadataProvider.GetFeatureInfo(typeof(DerivedClass)).Description, Is.EqualTo("DerivedDescription"));
        }

        [Test]
        public void Should_collect_redefined_description_from_derived_class()
        {
            Assert.That(_metadataProvider.GetFeatureInfo(typeof(OtherDerived)).Description, Is.EqualTo("derived"));
        }

        [ScenarioCategory("BaseC")]
        [FeatureDescription("BaseDescription")]
        class BaseClass
        {
            [ScenarioCategory("BaseMA")]
            public virtual void MethodA() { }
            [ScenarioCategory("BaseMB")]
            public virtual void MethodB() { }
        }

        [ScenarioCategory("DerivedC")]
        [FeatureDescription("DerivedDescription")]
        class DerivedClass : BaseClass
        {
            [ScenarioCategory("DerivedMB")]
            public override void MethodB()
            {
            }

            [ScenarioCategory("DerivedMC")]
            public void MethodC()
            {
            }
        }

        [FeatureDescription("base")]
        class OtherBase { }
        [FeatureDescription("derived")]
        class OtherDerived : OtherBase { }
    }
}