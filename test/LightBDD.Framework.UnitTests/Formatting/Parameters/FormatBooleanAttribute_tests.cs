using System;
using System.Globalization;
using LightBDD.Framework.Formatting.Parameters;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting.Parameters
{
    [TestFixture]
    public class FormatBooleanAttribute_tests
    {
        [Test]
        public void It_should_format_boolean()
        {
            var attribute = new FormatBooleanAttribute("on", "off");
            Assert.That(attribute.Format(CultureInfo.InvariantCulture, true), Is.EqualTo("on"));
            Assert.That(attribute.Format(CultureInfo.InvariantCulture, false), Is.EqualTo("off"));
        }

        [Test]
        public void It_should_throw_if_value_is_not_boolean()
        {
            var attribute = new FormatBooleanAttribute("on", "off");
            Assert.Throws<InvalidCastException>(() => attribute.Format(CultureInfo.InvariantCulture, 32));
        }
    }
}