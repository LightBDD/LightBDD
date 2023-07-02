using System;
using System.Globalization;
using LightBDD.Framework.Formatting;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting
{
    [TestFixture]
    public class FormatBooleanAttribute_tests
    {
        private readonly ValueFormattingServiceStub _formattingService = new(CultureInfo.InvariantCulture);

        [Test]
        public void FormatValue_should_format_boolean()
        {
            var attribute = new FormatBooleanAttribute("on", "off");

            Assert.That(attribute.FormatValue(true, _formattingService), Is.EqualTo("on"));
            Assert.That(attribute.FormatValue(false, _formattingService), Is.EqualTo("off"));
        }

        [Test]
        public void FormatValue_should_throw_if_value_is_not_boolean()
        {
            var attribute = new FormatBooleanAttribute("on", "off");
            Assert.Throws<InvalidCastException>(() => attribute.FormatValue(32, _formattingService));
        }

        [Test]
        public void FormatValue_should_throw_if_value_is_null()
        {
            var attribute = new FormatBooleanAttribute("on", "off");
            Assert.Throws<ArgumentNullException>(() => attribute.FormatValue(null, _formattingService));
        }

        [Test]
        public void CanFormat_should_accept_boolean_type()
        {
            var attribute = new FormatBooleanAttribute("yes", "no");
            Assert.That(attribute.CanFormat(typeof(bool)), Is.True);
            Assert.That(attribute.CanFormat(typeof(object)), Is.False);
        }
    }
}