using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LightBDD.Framework.Formatting.Values;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting.Values
{
    [TestFixture]
    public class CollectionFormatter_tests
    {
        [Test]
        public void FormatValue_should_format_collections_using_service_to_format_items()
        {
            var stub = new ValueFormattingServiceStub(CultureInfo.InvariantCulture, "-{0}-");
            var formatter = new CollectionFormatter();
            var actual = formatter.FormatValue(Enumerable.Range(0, 5), stub);
            Assert.That(actual, Is.EqualTo("-0-, -1-, -2-, -3-, -4-"));
        }

        [Test]
        public void CollectionFormatter_should_allow_custom_formatting()
        {
            var stub = new ValueFormattingServiceStub(CultureInfo.InvariantCulture);
            var formatter = new CollectionFormatter("[{0}]", " | ");
            var actual = formatter.FormatValue(Enumerable.Range(0, 5), stub);
            Assert.That(actual, Is.EqualTo("[0 | 1 | 2 | 3 | 4]"));
        }

        [Test]
        public void CollectionFormatter_should_format_empty_collections()
        {
            var stub = new ValueFormattingServiceStub(CultureInfo.InvariantCulture);
            var formatter = new CollectionFormatter();
            var actual = formatter.FormatValue(Enumerable.Empty<int>(), stub);
            Assert.That(actual, Is.EqualTo("<empty>"));
        }

        [Test]
        public void CanFormat_should_accept_any_type_implementing_IEnumerable()
        {
            var formatter = new CollectionFormatter();
            Assert.That(formatter.CanFormat(typeof(IEnumerable)), Is.True);
            Assert.That(formatter.CanFormat(typeof(int[])), Is.True);
            Assert.That(formatter.CanFormat(typeof(List<char>)), Is.True);
            Assert.That(formatter.CanFormat(Mock.Of<IEnumerable>().GetType()), Is.True);
        }
    }
}