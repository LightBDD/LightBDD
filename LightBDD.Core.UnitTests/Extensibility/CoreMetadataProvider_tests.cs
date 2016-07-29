using System;
using System.Globalization;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using LightBDD.Formatting.Parameters;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class CoreMetadataProvider_tests
    {
        private CoreMetadataProvider _metadataProvider;

        [SetUp]
        public void SetUp()
        {
            _metadataProvider = new TestMetadataProvider(new DefaultNameFormatter());
        }

        [Test]
        public void It_should_throw_if_nameFormatter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new TestMetadataProvider(null));
        }

        [Test]
        public void GetFeatureInfo_should_capture_feature_information_from_type()
        {
            var featureInfo = _metadataProvider.GetFeatureInfo(typeof(Feature_type));
            Assert.That(featureInfo.Name.ToString(), Is.EqualTo("Feature type"));
            Assert.That(featureInfo.Name.Parameters, Is.Empty);
            Assert.That(featureInfo.Description, Is.EqualTo("description"));
            Assert.That(featureInfo.Labels, Is.EqualTo(new[] { "l1", "l2" }));
        }

        [Test]
        public void GetScenarioName_should_capture_scenario_name_from_method()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method));
            var scenarioName = _metadataProvider.GetScenarioName(method);

            Assert.That(scenarioName.ToString(), Is.EqualTo("Some method"));
            Assert.That(scenarioName.Parameters, Is.Empty);
        }

        [Test]
        public void GetScenarioLabels_should_capture_scenario_labels_from_method()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method));
            Assert.That(_metadataProvider.GetScenarioLabels(method), Is.EqualTo(new[] { "s1", "s2" }));
        }

        [Test]
        public void GetScenarioCategories_should_capture_scenario_categories_from_method()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method));
            Assert.That(_metadataProvider.GetScenarioCategories(method), Is.EqualTo(new[] { "c1", "c2" }));
        }

        [Test]
        public void GetStepName_should_capture_name_from_step_descriptor_but_leave_parameters_unknown()
        {
            var descriptor = new StepDescriptor(
                "given",
                nameof(Feature_type.Some_step_with_argument),
                (o, a) => Task.CompletedTask,
                ParameterDescriptor.FromConstant(ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_argument), 5));

            var stepName = _metadataProvider.GetStepName(descriptor, null);
            Assert.That(stepName.StepTypeName, Is.EqualTo("GIVEN"));
            Assert.That(stepName.NameFormat, Is.EqualTo("Some step with argument \"{0}\""));
            Assert.That(stepName.ToString(), Is.EqualTo("GIVEN Some step with argument \"<?>\""));
        }

        [Test]
        public void GetStepParameterFormatter_should_capture_parameter_formatters()
        {
            var parameter1 = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_argument);
            var parameter2 = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_formatted_argument);
            var formatter1 = _metadataProvider.GetStepParameterFormatter(parameter1);
            var formatter2 = _metadataProvider.GetStepParameterFormatter(parameter2);

            Assert.That(formatter1.Invoke(5), Is.EqualTo($"{5}"));
            Assert.That(formatter2.Invoke(3), Is.EqualTo($"--{3}--"));
        }

        [Test]
        public void GetStepParameterFormatter_should_throw_if_multiple_parameter_formatters_are_declared_on_parameter()
        {
            var parameter = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_incorrectly_formatted_argument);
            var ex = Assert.Throws<InvalidOperationException>(() => _metadataProvider.GetStepParameterFormatter(parameter));
            Assert.That(ex.Message, Is.EqualTo($"Parameter can contain only one attribute ParameterFormatterAttribute. Parameter: argument, Detected attributes: {nameof(CustomFormatterAttribute)}, {nameof(FormatAttribute)}"));
        }

        [FeatureDescription("description"), Label("l1"), Label("l2")]
        class Feature_type
        {
            [Label("s1"), Label("s2")]
            [ScenarioCategory("c1"), ScenarioCategory("c2")]
            public void Some_method(int argument) { }

            public void Some_step_with_argument(int argument) { }
            public void Some_step_with_formatted_argument([CustomFormatter]int argument) { }
            public void Some_step_with_incorrectly_formatted_argument([CustomFormatter][Format("{0}")]int argument) { }
        }

        public class CustomFormatterAttribute : ParameterFormatterAttribute
        {
            public override string Format(CultureInfo culture, object parameter)
            {
                return string.Format(culture, "--{0}--", parameter);
            }
        }
    }
}