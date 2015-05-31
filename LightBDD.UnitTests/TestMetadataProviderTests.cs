using System.Linq;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.UnitTests
{
    [TestFixture]
    public class TestMetadataProviderTests
    {
        private TestMetadataProvider _subject;

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

        [Description("base")]
        class OtherBase { }
        [Description("derived")]
        class OtherDerived : OtherBase { }

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new TestableMetadataProvider();
        }

        #endregion

        [Test]
        public void Should_collect_categories_from_method_and_classes()
        {
            Assert.That(
                _subject.GetScenarioCategories(typeof(DerivedClass).GetMethod("MethodC")).ToArray(),
                Is.EqualTo(new[] { "BaseC", "DerivedC", "DerivedMC" }));
        }

        [Test]
        public void Should_collect_categories_from_method_with_base_and_classes()
        {
            Assert.That(
                _subject.GetScenarioCategories(typeof(DerivedClass).GetMethod("MethodB")).ToArray(),
                Is.EqualTo(new[] { "BaseC", "BaseMB", "DerivedC", "DerivedMB" }));
        }

        [Test]
        public void Should_collect_categories_from_base_method_and_base_class_only_if_method_is_declared_in_base()
        {
            Assert.That(
                _subject.GetScenarioCategories(typeof(DerivedClass).GetMethod("MethodA")).ToArray(),
                Is.EqualTo(new[] { "BaseC", "BaseMA" }));
        }

        [Test]
        public void Should_collect_redefined_feature_description_from_derived_class()
        {
            Assert.That(_subject.GetFeatureDescription(typeof(DerivedClass)), Is.EqualTo("DerivedDescription"));
        }

        [Test]
        public void Should_collect_redefined_description_from_derived_class()
        {
            Assert.That(_subject.GetFeatureDescription(typeof(OtherDerived)), Is.EqualTo("derived"));
        }

        [Test]
        [TestCase("given something", "GIVEN", "something")]
        [TestCase("GiVeN something", "GIVEN", "something")]
        [TestCase("when something", "WHEN", "something")]
        [TestCase("then something", "THEN", "something")]
        [TestCase("setup something", "SETUP", "something")]
        [TestCase("and something", "AND", "something")]
        [TestCase("given", "", "given")]
        [TestCase("given  ", "", "given  ")]
        [TestCase("givensomething", "", "givensomething")]
        [TestCase("xgiven", "", "xgiven")]
        public void By_default_should_get_step_type_from_formatted_name_properly_according_to_standard_GWT_keywords(string formattedName, string expectedType, string expectedName)
        {
            var type = _subject.GetStepTypeNameFromFormattedStepName(ref formattedName);
            Assert.That(type, Is.EqualTo(expectedType), "type");
            Assert.That(formattedName, Is.EqualTo(expectedName), "name");
        }

        [Test]
        [TestCase(null, "abc", null)]
        [TestCase("abc", null, "abc")]
        [TestCase("", "", "")]
        [TestCase("abc", "abc", "AND")]
        [TestCase("abc", "aBc", "AND")]
        [TestCase("abC", "aBc", "AND")]
        [TestCase("abc", "abcd", "abc")]
        [TestCase("abc", "ab", "abc")]
        [TestCase("one", "two", "one")]
        public void By_default_should_normalize_step_type_name_to_AND_if_type_is_repeated(string currentName, string lastName, string expectedName)
        {
            Assert.That(_subject.NormalizeStepTypeName(currentName, lastName), Is.EqualTo(expectedName));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \r\n\t")]
        public void Should_disable_normalization_if_replacementString_is_empty(string repeatedStepReplacement)
        {
            _subject = new TestableMetadataProvider(new[] { "given", "when", "then" }, repeatedStepReplacement);
            Assert.That(_subject.NormalizeStepTypeName("abc", "abc"), Is.EqualTo("abc"));
        }

        [Test]
        [TestCase("call something", "CALL", "something")]
        [TestCase("CaLl something", "CALL", "something")]
        [TestCase("invoke something", "INVOKE", "something")]
        [TestCase("then something", "", "then something")]
        public void Should_allow_to_reconfigure_GetStepTypeNameFromFormattedStepName(string formattedName, string expectedType, string expectedName)
        {
            _subject = new TestableMetadataProvider(new[] { "call", "invoke" }, "");

            var type = _subject.GetStepTypeNameFromFormattedStepName(ref formattedName);
            Assert.That(type, Is.EqualTo(expectedType), "type");
            Assert.That(formattedName, Is.EqualTo(expectedName), "name");
        }

        [Test]
        public void Should_initialize_object_with_default_values()
        {
            Assert.That(_subject.RepeatedStepReplacement, Is.EqualTo("and"));
            Assert.That(_subject.PredefinedStepTypes, Is.EquivalentTo(new[] { "given", "when", "then", "and", "setup" }));
        }
    }
}