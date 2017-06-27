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
        [TestCase("", null, "<null>")]
        public void It_should_use_specified_format(string culture, object value, string formattedValue)
        {
            var attribute = new FormatAttribute("--{0}--");
            Assert.That(attribute.Format(new CultureInfo(culture), value), Is.EqualTo($"--{formattedValue}--"));
        }
    }
}
