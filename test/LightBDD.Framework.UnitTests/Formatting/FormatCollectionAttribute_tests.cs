using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Formatting;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting
{
    [TestFixture]
    public class FormatCollectionAttribute_tests
    {
        [Test]
        public void FormatValue_should_format_collection()
        {
            var attribute = new FormatCollectionAttribute();
            var collection = new object[] { 5, 55.5, new DateTime(2016, 05, 14) };
            Assert.That(
                attribute.FormatValue(collection, ToFormattingService(CultureInfo.InvariantCulture)),
                Is.EqualTo("5, 55.5, 05/14/2016 00:00:00"));

            var customCultureInfo = new CultureInfo("PL");
            Assert.That(
                attribute.FormatValue(collection, ToFormattingService(customCultureInfo)),
                Is.EqualTo(string.Join(", ", collection.Select(c => string.Format(customCultureInfo, "{0}", c)))));
        }

        [Test]
        public void FormatValue_should_throw_if_collection_is_null()
        {
            var attribute = new FormatCollectionAttribute();
            var formattingService = ToFormattingService(CultureInfo.InvariantCulture);

            Assert.Throws<ArgumentNullException>(() => attribute.FormatValue(null, formattingService));
        }

        [Test]
        public void FormatValue_should_throw_if_no_collection_is_given()
        {
            var attribute = new FormatCollectionAttribute();
            var formattingService = ToFormattingService(CultureInfo.InvariantCulture);

            Assert.Throws<InvalidCastException>(() => attribute.FormatValue(new object(), formattingService));
        }

        [Test]
        public void FormatValue_should_format_collection_with_custom_formattings()
        {
            var attribute = new FormatCollectionAttribute("|", "<{0}>");
            var collection = new object[] { 5, 55.5, new DateTime(2016, 05, 14) };
            Assert.That(
                attribute.FormatValue(collection, ToFormattingService(CultureInfo.InvariantCulture)),
                Is.EqualTo("<5>|<55.5>|<05/14/2016 00:00:00>"));

            var customCultureInfo = new CultureInfo("PL");
            Assert.That(
                attribute.FormatValue(collection, ToFormattingService(customCultureInfo)),
                Is.EqualTo(string.Join("|", collection.Select(c => string.Format(customCultureInfo, "<{0}>", c)))));
        }

        [Test]
        public void CanFormat_should_accept_any_enumerable_type()
        {
            var attribute = new FormatCollectionAttribute();
            Assert.That(attribute.CanFormat(typeof(IEnumerable)), Is.True);
            Assert.That(attribute.CanFormat(typeof(object)), Is.False);
        }

        private static IValueFormattingService ToFormattingService(CultureInfo culture)
        {
            return new ValueFormattingServiceStub(culture);
        }
    }
}