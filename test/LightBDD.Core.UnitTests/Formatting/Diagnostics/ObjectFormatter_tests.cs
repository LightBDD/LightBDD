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

        class BasicTypes
        {
            public TimeSpan? Span { get; set; }
            public DateTime? DateTime { get; set; }
            public DateTimeOffset? DateTimeOffset { get; set; }
            public Guid? Guid { get; set; }
            public decimal Decimal { get; set; }
            public long? NullableLong { get; set; }
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
            Assert.That(actual, Is.EqualTo("ComplexType: { StringField=\"test!\" Int=5 Flag=True ReadonlyFloat=3.14 Array=[ Local Unspecified Utc ] Dictionary=[ { Key=\"abc\" Value={ StringField=null Int=0 Flag=False ReadonlyFloat=3.14 Array={...} Dictionary={...} Span=null } } ] Span=\"1.00:00:00\" }"));
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
            Assert.That(actual.Replace("\r", ""), Is.EqualTo(@"ComplexType: { StringField=""test!"" Int=5 Flag=True ReadonlyFloat=3.14 Array=[ Local Unspecified Utc ] Dictionary=[ {...} ] Span=""12:00:00"" }
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

        [Test]
        public void It_should_format_basic_types_properly()
        {
            var obj = new BasicTypes
            {
                DateTimeOffset = new DateTimeOffset(2022,01,02,03,04,05,TimeSpan.FromHours(3)),
                DateTime = new DateTime(2022,01,02,03,04,05,DateTimeKind.Utc),
                Decimal = (decimal)3.14,
                Guid = Guid.Parse("8bdcb730-6d08-409d-8170-12da52cb71e1"),
                NullableLong = 34,
                Span = TimeSpan.FromTicks(101427461000),
            };
            var actual = ObjectFormatter.Dump(obj);
            Assert.That(actual.Replace("\r", ""), Is.EqualTo("BasicTypes: { Span=\"02:49:02.7461000\" DateTime=\"2022-01-02T03:04:05.0000000Z\" DateTimeOffset=\"2022-01-02T03:04:05.0000000+03:00\" Guid=\"8bdcb730-6d08-409d-8170-12da52cb71e1\" Decimal=\"3.14\" NullableLong=34 }".Replace("\r", "")));
        }
    }
}
