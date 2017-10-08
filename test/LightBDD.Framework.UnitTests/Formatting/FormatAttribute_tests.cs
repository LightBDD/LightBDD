using System.Globalization;
using LightBDD.Framework.Formatting;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting
{
    [TestFixture]
    public class FormatAttribute_tests
    {
        [Test]
        [TestCase("", 55.5, "55.5")]
        [TestCase("PL", 55.5, "55,5")]
        [TestCase("", "", "")]
        public void It_should_use_specified_format(string culture, object value, string formattedValue)
        {
            var attribute = new FormatAttribute("--{0}--");
            var formattingService = new ValueFormattingServiceStub(new CultureInfo(culture));
            Assert.That(attribute.FormatValue(value, formattingService), Is.EqualTo($"--{formattedValue}--"));
        }
    }
}
