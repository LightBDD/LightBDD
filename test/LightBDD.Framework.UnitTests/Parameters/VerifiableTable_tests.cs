using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Parameters
{
    [TestFixture]
    public class VerifiableTable_tests
    {
        class Base
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public virtual int Virtual { get; set; }
            public string Field;
        }

        class Derived : Base
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public override int Virtual { get; set; }
        }

        struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_class_collection()
        {
            TestCollectionAsVerifiableTable(new[]
                {
                    new Derived {Field = "a", Name = "b", Text = "c", Value = "d", Virtual = 5},
                    new Derived {Field = "a2", Name = "b2", Text = "c2", Value = "d2", Virtual = 10}
                },
                new[] { "Field", "Name", "Text", "Value", "Virtual" },
                1,
                new[] { ColumnValue.From("a2"), ColumnValue.From("b2"), ColumnValue.From("c2"), ColumnValue.From("d2"), ColumnValue.From(10) });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_struct_collection()
        {
            TestCollectionAsVerifiableTable(
                new[]
                {
                    new Point(2, 3),
                    new Point(3, 4)
                },
                new[] { "X", "Y" },
                1,
                new[] { ColumnValue.From(3), ColumnValue.From(4) });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_ValueTuple_collection()
        {
            TestCollectionAsVerifiableTable(
                new[]
                {
                    (name: "Joe", surname: "Smith"),
                    (name: "John", surname: "Jackson")
                },
                new[] { "Item1", "Item2" },
                1,
                new[] { ColumnValue.From("John"), ColumnValue.From("Jackson") });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_unnamed_ValueTuple_collection()
        {
            TestCollectionAsVerifiableTable(
                new[]
                {
                    ( "Joe",  "Smith"),
                    ( "John",  "Jackson")
                },
                new[] { "Item1", "Item2" },
                1,
                new[] { ColumnValue.From("John"), ColumnValue.From("Jackson") });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_Tuple_collection()
        {
            TestCollectionAsVerifiableTable(
                new[]
                {
                    Tuple.Create("Joe", "Smith"),
                    Tuple.Create("John", "Jackson")
                },
                new[] { "Item1", "Item2" },
                1,
                new[] { ColumnValue.From("John"), ColumnValue.From("Jackson") });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_Int_collection()
        {
            TestCollectionAsVerifiableTable(
                new[] { 1, 2, 3 },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From(2) });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_String_collection()
        {
            TestCollectionAsVerifiableTable(
                new[] { "t1", "t2", "t3" },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From("t2") });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_Object_collection()
        {
            TestCollectionAsVerifiableTable(
                new object[] { "t1", 2, 'c' },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From(2) });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_multi_dimensional_collection()
        {
            TestCollectionAsVerifiableTable(
                new[]
                {
                    new[]{1,2},
                    new[]{3,4,5},
                    new[]{6,7,8,9}
                },
                new[] { "Length", "[0]", "[1]", "[2]", "[3]" },
                1,
                new[] { ColumnValue.From(3), ColumnValue.From(3), ColumnValue.From(4), ColumnValue.From(5), ColumnValue.None });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_ExpandoObject_collection()
        {
            var json = @"[
{""name"":""John""},
{""name"":""Sarah"",""surname"":""Smith""},
{""name"":""Joe"",""surname"":""Smith"",""age"":27}
]";
            TestCollectionAsVerifiableTable(
                JsonConvert.DeserializeObject<ExpandoObject[]>(json),
                new[] { "age", "name", "surname" },
                1,
                new[] { ColumnValue.None, ColumnValue.From("Sarah"), ColumnValue.From("Smith") });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_Dictionary()
        {
            var input = new Dictionary<string, Point>
            {
                {"key1", new Point(3, 5)},
                {"key2", new Point(2, 7)}
            };
            var table = input.AsVerifiableTable();
            AssertColumnNames(table, "Key", "X", "Y");
            Assert.That(table.Columns[0].IsKey, Is.True);
            Assert.That(table.Columns[1].IsKey, Is.False);
            Assert.That(table.Columns[2].IsKey, Is.False);

            AssertValues(table, input.First(), ColumnValue.From("key1"), ColumnValue.From(3), ColumnValue.From(5));
        }

        [Test]
        public void AsVerifiableTable_should_allow_defining_custom_columns()
        {
            var input = new[]
            {
                new {Id=123,X=5,Y=7}
            };

            var table = input.AsVerifiableTable(r => r
                .WithKey(v => v.Id)
                .WithKey("Secondary", v => $"{v.Id}_{v.X}_{v.Y}")
                .WithColumn(v => v.X)
                .WithColumn(v => v.Y)
                .WithColumn("Sum", v => v.X + v.Y));

            AssertColumnNames(table,"Id","Secondary", "X", "Y", "Sum");
            Assert.That(table.Columns[0].IsKey, Is.True);
            Assert.That(table.Columns[1].IsKey, Is.True);
            Assert.That(table.Columns[2].IsKey, Is.False);
            Assert.That(table.Columns[3].IsKey, Is.False);
            Assert.That(table.Columns[4].IsKey, Is.False);

            AssertValues(table, input[0], ColumnValue.From(123), ColumnValue.From("123_5_7"), ColumnValue.From(5), ColumnValue.From(7), ColumnValue.From(12));
        }

        private static void TestCollectionAsVerifiableTable<T>(T[] input, string[] expectedColumns, int index, ColumnValue[] expectedValues)
        {
            var table = input.AsVerifiableTable();
            Assert.That(table.Columns.All(x => !x.IsKey), Is.True);
            AssertColumnNames(table, expectedColumns);
            AssertValues(table, input[index], expectedValues);
        }

        private static void AssertValues<T>(VerifiableTable<T> table, T row, params ColumnValue[] expectedValues)
        {
            Assert.That(table.Columns.Select(c => c.GetValue(row)).ToArray(), Is.EqualTo(expectedValues));
        }

        private static void AssertColumnNames<T>(VerifiableTable<T> table, params string[] expectedColumns)
        {
            Assert.That(table.Columns.Select(c => c.Name).ToArray(), Is.EqualTo(expectedColumns));
        }
    }
}