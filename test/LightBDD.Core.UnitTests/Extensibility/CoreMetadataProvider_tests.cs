using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Formatting;
using LightBDD.UnitTests.Helpers.TestableIntegration;
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
        public void GetScenarioName_should_capture_parameterless_scenario_name_from_descriptor()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method_without_arguments));
            var scenarioName = _metadataProvider.GetScenarioName(new ScenarioDescriptor(method, new object[0]));

            Assert.That(scenarioName.ToString(), Is.EqualTo("Some method without arguments"));
            Assert.That(scenarioName.Parameters, Is.Empty);
        }

        [Test]
        public void GetScenarioName_should_capture_parameterized_scenario_name_from_descriptor_honoring_parameter_formatters()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method_with_argument_arg1_and_arg2));
            var scenarioName = _metadataProvider.GetScenarioName(new ScenarioDescriptor(method, new object[] { 5, "text" }));

            Assert.That(scenarioName.NameFormat, Is.EqualTo("Some method with argument arg1 \"{0}\" and arg2 \"{1}\""));
            Assert.That(scenarioName.ToString(), Is.EqualTo("Some method with argument arg1 \"--5--\" and arg2 \"text\""));
            Assert.That(scenarioName.Parameters, Is.Not.Empty);

            Assert.That(scenarioName.Parameters.Select(p => p.FormattedValue).ToArray(), Is.EqualTo(new[] { "--5--", "text" }));
        }

        [Test]
        public void GetScenarioName_should_throw_if_multiple_parameter_formatters_are_declared_on_parameter()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_step_with_incorrectly_formatted_argument));
            var ex = Assert.Throws<InvalidOperationException>(() => _metadataProvider.GetScenarioName(new ScenarioDescriptor(method, new object[] { 5 })));
            Assert.That(ex.Message, Is.EqualTo($"Unable to obtain scenario name for method Some_step_with_incorrectly_formatted_argument: Parameter can contain only one attribute ParameterFormatterAttribute. Parameter: argument, Detected attributes: {nameof(CustomFormatterAttribute)}, {nameof(FormatAttribute)}"));
        }

        [Test]
        public void GetScenarioName_should_capture_parameterized_scenario_name_from_descriptor_allowing_unknown_arguments()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method_with_argument_arg1_and_arg2));
            var scenarioName = _metadataProvider.GetScenarioName(new ScenarioDescriptor(method, null));

            Assert.That(scenarioName.ToString(), Is.EqualTo("Some method with argument arg1 and arg2"));
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
                (o, a) => Task.FromResult(StepResultDescriptor.Default),
                ParameterDescriptor.FromConstant(ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_argument), 5));

            var stepName = _metadataProvider.GetStepName(descriptor, null);
            Assert.That(stepName.StepTypeName.ToString(), Is.EqualTo("GIVEN"));
            Assert.That(stepName.NameFormat, Is.EqualTo("Some step with argument \"{0}\""));
            Assert.That(stepName.ToString(), Is.EqualTo("GIVEN Some step with argument \"<?>\""));
        }

        [Test]
        public void GetStepParameterFormatter_should_capture_parameter_formatters()
        {
            var parameter1 = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_argument);
            var parameter2 = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_formatted_argument);
            var formatter1 = _metadataProvider.GetParameterFormatter(parameter1);
            var formatter2 = _metadataProvider.GetParameterFormatter(parameter2);

            Assert.That(formatter1.Invoke(5), Is.EqualTo($"{5}"));
            Assert.That(formatter2.Invoke(3), Is.EqualTo($"--{3}--"));
        }

        [Test]
        public void GetStepParameterFormatter_should_throw_if_multiple_parameter_formatters_are_declared_on_parameter()
        {
            var parameter = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_incorrectly_formatted_argument);
            var ex = Assert.Throws<InvalidOperationException>(() => _metadataProvider.GetParameterFormatter(parameter));
            Assert.That(ex.Message, Is.EqualTo($"Parameter can contain only one attribute ParameterFormatterAttribute. Parameter: argument, Detected attributes: {nameof(CustomFormatterAttribute)}, {nameof(FormatAttribute)}"));
        }

        [FeatureDescription("description"), Label("l1"), Label("l2")]
        class Feature_type
        {
            [Label("s1"), Label("s2")]
            [ScenarioCategory("c1"), ScenarioCategory("c2")]
            public void Some_method(int argument) { }

            public void Some_method_without_arguments() { }
            public void Some_method_with_argument_arg1_and_arg2([CustomFormatter]int arg1, string arg2) { }

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