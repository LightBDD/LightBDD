using System;
using System.Collections.Generic;
using LightBDD.Core.Formatting.Diagnostics;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Formatting.Diagnostics
{
    [TestFixture]
    [SetCulture("pl-PL")]
    public class ObjectFormatter_tests
    {
        class BaseType
        {
            public string StringField;
        }
        class ComplexType : BaseType
        {
            public int Int { get; set; }
            public bool Flag { get; set; }
            public float ReadonlyFloat => 3.14f;
            public DateTimeKind[] Array => new[] { DateTimeKind.Local, DateTimeKind.Unspecified, DateTimeKind.Utc };
            public Dictionary<string, ComplexType> Dictionary { get; } = new Dictionary<string, ComplexType>();
            public TimeSpan? Span { get; set; }
        }

        class Problematic
        {
            public string Prop => throw new InvalidOperationException("foo");
        }

        [Test]
        public void It_should_format_object()
        {
            var obj = new ComplexType
            {
                StringField = "test!",
                Int = 5,
                Flag = true,
                Span = TimeSpan.FromDays(1),
                Dictionary = { { "abc", new ComplexType { Dictionary = { { "def", new ComplexType() } } } } }
            };

            var actual = ObjectFormatter.Dump(obj);
            Assert.That(actual, Is.EqualTo("ComplexType: { StringField=\"test!\" Int=5 Flag=True ReadonlyFloat=3.14 Array=[ Local Unspecified Utc ] Dictionary=[ { Key=\"abc\" Value={ StringField=null Int=0 Flag=False ReadonlyFloat=3.14 Array={...} Dictionary={...} Span=null } } ] Span={ Ticks=864000000000 Days=1 Hours=0 Milliseconds=0 Minutes=0 Seconds=0 TotalDays=1 TotalHours=24 TotalMilliseconds=86400000 TotalMinutes=1440 TotalSeconds=86400 } }"));
        }

        [Test]
        public void It_should_format_object_collection()
        {
            var items = new object[]
            {
                new ComplexType
                {
                    StringField = "test!",
                    Int = 5,
                    Flag = true,
                    Span = TimeSpan.FromDays(0.5),
                    Dictionary = { { "abc", null } }
                },
                new BaseType { StringField = "123" },
                3.14,
                false
            };

            var actual = ObjectFormatter.DumpMany(items);
            Assert.That(actual.Replace("\r", ""), Is.EqualTo(@"ComplexType: { StringField=""test!"" Int=5 Flag=True ReadonlyFloat=3.14 Array=[ Local Unspecified Utc ] Dictionary=[ {...} ] Span={ Ticks=432000000000 Days=0 Hours=12 Milliseconds=0 Minutes=0 Seconds=0 TotalDays=0.5 TotalHours=12 TotalMilliseconds=43200000 TotalMinutes=720 TotalSeconds=43200 } }
BaseType: { StringField=""123"" }
Double: 3.14
Boolean: False".Replace("\r", "")));
        }

        [Test]
        public void It_should_format_problematic_property()
        {
            var actual = ObjectFormatter.Dump(new Problematic());
            Assert.That(actual, Is.EqualTo("Problematic: { Prop=!InvalidOperationException:\"foo\" }"));
        }
    }
}
