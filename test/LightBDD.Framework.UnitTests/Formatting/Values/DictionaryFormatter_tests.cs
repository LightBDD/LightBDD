using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using LightBDD.Framework.Formatting.Values;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting.Values
{
    [TestFixture]
    public class DictionaryFormatter_tests
    {
        [Test]
        public void FormatValue_should_format_dictionaries_ordered_by_key()
        {
            var stub = new ValueFormattingServiceStub(CultureInfo.InvariantCulture);
            var formatter = new DictionaryFormatter();

            var dictionary = new Dictionary<float, string>
            {
                {0, "a"},
                {-0.7f, "b"},
                {3.11f, "d"},
                {3.2f, "c"}
            };

            var actual = formatter.FormatValue(dictionary, stub);
            Assert.That(actual, Is.EqualTo("-0.7: b, 0: a, 3.11: d, 3.2: c"));
        }

        [Test]
        public void DictionaryFormatter_should_use_service_to_format_items()
        {
            var stub = new ValueFormattingServiceStub(CultureInfo.InvariantCulture, ">{0}");
            var formatter = new DictionaryFormatter();
            var dictionary = new Dictionary<string, string> {{"test", "a"}, {"other", "b"}};

            var actual = formatter.FormatValue(dictionary, stub);
            Assert.That(actual, Is.EqualTo(">other: >b, >test: >a"));
        }

        [Test]
        public void DictionaryFormatter_should_allow_custom_formatting()
        {
            var stub = new ValueFormattingServiceStub(CultureInfo.InvariantCulture);
            var formatter = new DictionaryFormatter("<{0}>","[{0}={1}]", " | ");
            var dictionary = new Dictionary<string, string> {{"test", "a"}, {"other", "b"}};

            var actual = formatter.FormatValue(dictionary, stub);
            Assert.That(actual, Is.EqualTo("<[other=b] | [test=a]>"));
        }

        [Test]
        public void CanFormat_should_accept_any_type_implementing_IDictionary()
        {
            var formatter = new DictionaryFormatter();
            Assert.That(formatter.CanFormat(typeof(IDictionary)), Is.True);
            Assert.That(formatter.CanFormat(typeof(Dictionary<string, object>)), Is.True);
            Assert.That(formatter.CanFormat(typeof(ConcurrentDictionary<int, char>)), Is.True);
            Assert.That(formatter.CanFormat(Mock.Of<IDictionary>().GetType()), Is.True);
        }
    }
}