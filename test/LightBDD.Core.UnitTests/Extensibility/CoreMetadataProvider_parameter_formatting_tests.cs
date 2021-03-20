using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Formatting;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class CoreMetadataProvider_parameter_formatting_tests
    {
        private class Feature_type
        {
            public void Some_method_with_argument_arg1_and_arg2([CustomFormatter] int arg1, string arg2) { }
            public void Some_step_with_argument(int argument) { }
            public void Some_step_with_formatted_argument([CustomFormatter] int argument) { }
            public void Some_step_with_multiple_formatters_on_argument([CustomFormatter(Order = 1)][Format("{0}", Order = 2)] int argument) { }
        }

        private class CustomFormatterAttribute : ParameterFormatterAttribute
        {
            public override string FormatValue(object value, IValueFormattingService formattingService)
            {
                return string.Format(formattingService.GetCultureInfo(), "--{0}--", value);
            }
        }

        private static TestMetadataProvider GetMetadataProvider()
        {
            return new TestMetadataProvider(cfg => cfg.ValueFormattingConfiguration().RegisterFrameworkDefaultGeneralFormatters());
        }

        [Test]
        public void GetScenarioName_should_capture_parameterized_scenario_name_from_descriptor_honoring_parameter_formatters()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method_with_argument_arg1_and_arg2));
            var scenarioName = GetMetadataProvider().GetScenarioName(new ScenarioDescriptor(method, new object[] { 5, "text" }));

            Assert.That(scenarioName.NameFormat, Is.EqualTo("Some method with argument arg1 \"{0}\" and arg2 \"{1}\""));
            Assert.That(scenarioName.ToString(), Is.EqualTo("Some method with argument arg1 \"--5--\" and arg2 \"text\""));
            Assert.That(scenarioName.Parameters, Is.Not.Empty);

            Assert.That(scenarioName.Parameters.Select(p => p.FormattedValue).ToArray(), Is.EqualTo(new[] { "--5--", "text" }));
            Assert.That(scenarioName.Parameters.Select(p => p.Name).ToArray(), Is.EqualTo(new[] { "arg1", "arg2" }));
        }

        [Test]
        public void GetScenarioName_should_capture_parameterized_scenario_name_from_descriptor_with_multiple_parameter_formatters()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_step_with_multiple_formatters_on_argument));
            var scenarioName = GetMetadataProvider().GetScenarioName(new ScenarioDescriptor(method, new object[] { 5 }));
            Assert.That(scenarioName.ToString(), Is.EqualTo("Some step with multiple formatters on argument \"--5--\""));
        }

        [Test]
        public void GetValueFormattingServiceFor_should_capture_parameter_formatters()
        {
            var parameter1 = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_argument);
            var parameter2 = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_formatted_argument);
            var formatter1 = GetMetadataProvider().GetValueFormattingServiceFor(parameter1);
            var formatter2 = GetMetadataProvider().GetValueFormattingServiceFor(parameter2);

            Assert.That(formatter1.FormatValue(5), Is.EqualTo($"{5}"));
            Assert.That(formatter2.FormatValue(3), Is.EqualTo($"--{3}--"));
        }

        [Test]
        public void GetValueFormattingServiceFor_should_honor_multiple_parameter_formatters()
        {
            var parameter = ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_multiple_formatters_on_argument);
            var formatter = GetMetadataProvider().GetValueFormattingServiceFor(parameter);
            Assert.That(formatter.FormatValue(3), Is.EqualTo("--3--"));
        }

        [Test]
        public void GetValueFormattingServiceFor_should_format_null()
        {
            void Step(string[] collection) { }

            var parameter = ParameterInfoHelper.GetMethodParameter<string[]>(Step);
            var formatter = GetMetadataProvider().GetValueFormattingServiceFor(parameter);
            Assert.That(formatter.FormatValue(null), Is.EqualTo("<null>"));
        }

        [Test]
        public void GetValueFormattingServiceFor_should_format_collection_honoring_null_values()
        {
            void Step(string[] collection) { }

            var parameter = ParameterInfoHelper.GetMethodParameter<string[]>(Step);
            var formatter = GetMetadataProvider().GetValueFormattingServiceFor(parameter);
            Assert.That(formatter.FormatValue(new[] { "abc", null, "def" }), Is.EqualTo("abc, <null>, def"));
        }

        [Test]
        public void GetValueFormattingServiceFor_should_format_dictionaries_honoring_null_values()
        {
            void Step(Dictionary<string, string> collection) { }

            var parameter = ParameterInfoHelper.GetMethodParameter<Dictionary<string, string>>(Step);
            var formatter = GetMetadataProvider().GetValueFormattingServiceFor(parameter);
            var dict = new Dictionary<string, string>
            {
                {"0", null},
                {"abc", null},
                {"def", "value"}
            };
            Assert.That(formatter.FormatValue(dict), Is.EqualTo("0: <null>, abc: <null>, def: value"));
        }

        [Test]
        public void GetValueFormattingServiceFor_should_honor_custom_formatters_then_explicit_then_formattable_then_general_then_ToString()
        {
            var testMetadataProvider = new TestMetadataProvider(cfg => cfg.ValueFormattingConfiguration()
                .RegisterFrameworkDefaultGeneralFormatters()
                .RegisterExplicit(typeof(bool), new FormatAttribute(">{0}<"))
                .RegisterExplicit(typeof(int), new FormatAttribute("i{0}"))
                .RegisterExplicit(typeof(MyFormattable2), new FormatAttribute("my-explicit2"))
                .RegisterGeneral(new MyStructFormatter()));

            var parameter = ParameterInfoHelper.GetMethodParameter<object[]>(Step_with_custom_formatters);
            var formatter = testMetadataProvider
                .GetValueFormattingServiceFor(parameter);

            var values = new object[]
            {
                null, // null
                5, // explicit int
                true, // [FormatBoolean] overriding explicit bool
                5.5, // general struct
                new MyClass(), // toString
                new MyFormattable1(), // [Format]
                new MyFormattable2(), // explicit formatter
                new MyFormattable3() // ISelfFormattable
            };
            Assert.That(formatter.FormatValue(values), Is.EqualTo("#<null> | #i5 | #On | #s5.5 | #my-class | #my-custom-format1 | #my-explicit2 | #my3"));
        }

        [Test]
        public void GetValueFormattingServiceFor_should_honor_custom_attribute_formatters_for_collection_items()
        {
            var parameter = ParameterInfoHelper.GetMethodParameter<bool[]>(Step_with_custom_formatter_for_collection_item);
            var formatter = GetMetadataProvider().GetValueFormattingServiceFor(parameter);
            Assert.That(formatter.FormatValue(new[] { true, false }), Is.EqualTo("On, Off"));
        }

        private void Step_with_custom_formatters([FormatCollection(" | ", "#{0}")][FormatBoolean("On", "Off")][Format("my-custom-format1", SupportedType = typeof(MyFormattable1))] object[] arg) { }
        private void Step_with_custom_formatter_for_collection_item([FormatBoolean("On", "Off")] bool[] arg) { }

        private class MyStructFormatter : IConditionalValueFormatter
        {
            public string FormatValue(object value, IValueFormattingService formattingService)
            {
                return string.Format(formattingService.GetCultureInfo(), "s{0}", value);
            }

            public bool CanFormat(Type type)
            {
                return type.GetTypeInfo().IsValueType;
            }
        }

        private class MyClass
        {
            public override string ToString()
            {
                return "my-class";
            }
        }

        private class MyFormattable1 : ISelfFormattable
        {
            public string Format(IValueFormattingService formattingService)
            {
                return "my1";
            }
        }
        private class MyFormattable2 : ISelfFormattable
        {
            public string Format(IValueFormattingService formattingService)
            {
                return "my2";
            }
        }
        private class MyFormattable3 : ISelfFormattable
        {
            public string Format(IValueFormattingService formattingService)
            {
                return "my3";
            }
        }
    }
}
