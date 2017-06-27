using System;
using System.Globalization;
using System.Linq;
using LightBDD.Framework.Formatting;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting
{
    [TestFixture]
    public class FormatCollectionAttribute_tests
    {
        [Test]
        public void It_should_format_collection()
        {
            var attribute = new FormatCollectionAttribute();
            var collection = new object[] { 5, 55.5, new DateTime(2016, 05, 14), null };
            Assert.That(attribute.Format(CultureInfo.InvariantCulture, collection), Is.EqualTo("5, 55.5, 05/14/2016 00:00:00, <null>"));

            var customCultureInfo = new CultureInfo("PL");
            Assert.That(attribute.Format(customCultureInfo, collection), Is.EqualTo(string.Join(", ", collection.Select(c => string.Format(customCultureInfo, "{0}", c ?? "<null>")))));
        }

        [Test]
        public void It_should_format_null_collection()
        {
            var attribute = new FormatCollectionAttribute();
            Assert.That(attribute.Format(CultureInfo.InvariantCulture, null), Is.EqualTo("<null>"));
        }

        [Test]
        public void It_should_format_collection_with_custom_formattings()
        {
            var attribute = new FormatCollectionAttribute("|", "<{0}>");
            var collection = new object[] { 5, 55.5, new DateTime(2016, 05, 14), null };
            Assert.That(attribute.Format(CultureInfo.InvariantCulture, collection), Is.EqualTo("<5>|<55.5>|<05/14/2016 00:00:00>|<<null>>"));

            var customCultureInfo = new CultureInfo("PL");
            Assert.That(attribute.Format(customCultureInfo, collection), Is.EqualTo(string.Join("|", collection.Select(c => string.Format(customCultureInfo, "<{0}>", c ?? "<null>")))));
        }
    }
}