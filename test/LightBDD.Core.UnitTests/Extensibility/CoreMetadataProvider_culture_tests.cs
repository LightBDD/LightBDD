using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
using System.Globalization;
using LightBDD.ScenarioHelpers;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class CoreMetadataProvider_culture_tests
    {
        [Test]
        [TestCase("pl-PL", 3.14, "3,14")]
        [TestCase("en-US", 3.14, "3.14")]
        public void It_should_format_step_parameters_with_specified_formatter(string cultureInfo, double parameter, string expectedFormattedParameter)
        {
            var parameterInfo = ParameterInfoHelper.GetMethodParameter<double>(Step_with_parameter);
            var metadataProvider = TestMetadataProvider.Create(cfg =>
                cfg.RegisterCultureInfoProvider(x => x.Use(new TestCultureInfoProvider(new CultureInfo(cultureInfo)))));
            var formatter = metadataProvider.GetValueFormattingServiceFor(parameterInfo);

            Assert.That(formatter.FormatValue(parameter), Is.EqualTo(expectedFormattedParameter));
        }

        private static void Step_with_parameter(double value) { }

        private class TestCultureInfoProvider : ICultureInfoProvider
        {
            private readonly CultureInfo _cultureInfo;

            public TestCultureInfoProvider(CultureInfo cultureInfo)
            {
                _cultureInfo = cultureInfo;
            }

            public CultureInfo GetCultureInfo()
            {
                return _cultureInfo;
            }
        }
    }
}
