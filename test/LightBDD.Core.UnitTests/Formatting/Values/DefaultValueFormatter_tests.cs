using System.Globalization;
using LightBDD.Core.Formatting.Values;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Formatting.Values
{
    [TestFixture]
    public class DefaultValueFormatter_tests
    {
        [Test]
        [TestCase("PL", 5.5, "5,5")]
        [TestCase("", 5.5, "5.5")]
        public void FormatValue_should_format_value_using_configured_culture(string culture, object value, string expected)
        {
            var cultureInfo = new CultureInfo(culture);
            var actual = DefaultValueFormatter.Instance.FormatValue(value, GetValueFormattingService(cultureInfo));
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static IValueFormattingService GetValueFormattingService(CultureInfo culture)
        {
            var formattingService = Mock.Of<IValueFormattingService>();
            Mock.Get(formattingService).Setup(x => x.GetCultureInfo()).Returns(culture);
            return formattingService;
        }
    }
}
