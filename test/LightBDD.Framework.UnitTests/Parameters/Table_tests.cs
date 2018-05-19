using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Parameters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Parameters
{
    [TestFixture]
    public class Table_tests
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
        public void AsTable_should_infer_columns_from_class_collection()
        {
            TestCollectionAsTable(new[]
                {
                    new Derived {Field = "a", Name = "b", Text = "c", Value = "d", Virtual = 5},
                    new Derived {Field = "a2", Name = "b2", Text = "c2", Value = "d2", Virtual = 10}
                },
                new[] { "Field", "Name", "Text", "Value", "Virtual" },
                1,
                new[] { ColumnValue.From("a2"), ColumnValue.From("b2"), ColumnValue.From("c2"), ColumnValue.From("d2"), ColumnValue.From(10) });
        }

        [Test]
        public void AsTable_should_infer_columns_from_struct_collection()
        {
            TestCollectionAsTable(
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
        public void AsTable_should_infer_columns_from_ValueTuple_collection()
        {
            TestCollectionAsTable(
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
        public void AsTable_should_infer_columns_from_unnamed_ValueTuple_collection()
        {
            TestCollectionAsTable(
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
        public void AsTable_should_infer_columns_from_Tuple_collection()
        {
            TestCollectionAsTable(
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
        public void AsTable_should_infer_columns_from_Int_collection()
        {
            TestCollectionAsTable(
                new[] { 1, 2, 3 },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From(2) });
        }

        [Test]
        public void AsTable_should_infer_columns_from_String_collection()
        {
            TestCollectionAsTable(
                new[] { "t1", "t2", "t3" },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From("t2") });
        }

        [Test]
        public void AsTable_should_infer_columns_from_Object_collection()
        {
            TestCollectionAsTable(
                new object[] { "t1", 2, 'c' },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From(2) });
        }

        [Test]
        public void AsTable_should_infer_columns_from_multi_dimensional_collection()
        {
            TestCollectionAsTable(
                new[]
                {
                    new[]{1,2},
                    new[]{3,4,5},
                    new[]{6,7,8,9}
                },
                new[] { "[0]", "[1]", "[2]", "[3]" },
                1,
                new[] { ColumnValue.From(3), ColumnValue.From(4), ColumnValue.From(5), ColumnValue.None });
        }

        [Test]
        public void AsTable_should_infer_columns_from_ExpandoObject_collection()
        {
            var json = @"[
{""name"":""John""},
{""name"":""Sarah"",""surname"":""Smith""},
{""name"":""Joe"",""surname"":""Smith"",""age"":27}
]";
            TestCollectionAsTable(
                JsonConvert.DeserializeObject<ExpandoObject[]>(json),
                new[] { "age", "name", "surname" },
                1,
                new[] { ColumnValue.None, ColumnValue.From("Sarah"), ColumnValue.From("Smith") });
        }

        [Test]
        public void AsVerifiableTable_should_infer_columns_from_dynamic_collection_of_unified_item_types()
        {
            var values = new[]
                {
                    new Point(2, 3),
                    new Point(3, 4)
                }
                .Cast<dynamic>()
                .ToArray();

            TestCollectionAsTable(
                values,
                new[] { "X", "Y" },
                1,
                new[] { ColumnValue.From(3), ColumnValue.From(4) });
        }

        [Test]
        public void AsTable_should_infer_columns_from_Dictionary()
        {
            var input = new Dictionary<string, Point>
            {
                {"key1", new Point(3, 5)},
                {"key2", new Point(2, 7)}
            };
            var table = input.AsTable();
            AssertColumnNames(table, "Key", "X", "Y");

            AssertValues(table, input.First(), ColumnValue.From("key1"), ColumnValue.From(3), ColumnValue.From(5));
        }

        [Test]
        public void AsTable_should_allow_defining_custom_columns()
        {
            var input = new[]
            {
                new {Id = 123, X = 5, Y = 7}
            };

            var table = input.AsTable(r => r
                .WithColumn(v => v.Id)
                .WithColumn(v => v.X)
                .WithColumn(v => v.Y)
                .WithColumn("Sum", v => v.X + v.Y));

            AssertColumnNames(table, "Id", "X", "Y", "Sum");
            AssertValues(table, input[0], ColumnValue.From(123), ColumnValue.From(5), ColumnValue.From(7), ColumnValue.From(12));
        }

        [Test]
        public void AsVerifiableTable_should_allow_inferring_columns_with_custom_override()
        {
            var input = new[]
            {
                new {Id = 123, X = 5, Y = 7}
            };

            var table = input.AsTable(r => r
                .WithInferredColumns()
                .WithColumn("Sum", v => v.X + v.Y)
                .WithColumn("Y", v => v.Y * 2));

            AssertColumnNames(table, "Id", "X", "Y", "Sum");
            AssertValues(table, input[0], ColumnValue.From(123), ColumnValue.From(5), ColumnValue.From(14), ColumnValue.From(12));
        }

        [Test]
        public void Table_should_allow_enumerating_through_provided_rows()
        {
            var input = new[]
            {
                new {Id = 6, X = 2, Y = 5},
                new {Id = 7, X = 3, Y = 7}
            };
            Assert.That(input.AsTable().AsEnumerable(), Is.EqualTo(input));
        }

        [Test]
        public void Table_should_allow_accessing_provided_rows_via_indexer()
        {
            var input = new[]
            {
                new {Id = 6, X = 2, Y = 5},
                new {Id = 7, X = 3, Y = 7}
            };
            var table = input.AsTable();
            Assert.That(table[0], Is.EqualTo(input[0]));
            Assert.That(table[1], Is.EqualTo(input[1]));
        }

        private static void TestCollectionAsTable<T>(T[] input, string[] expectedColumns, int index, ColumnValue[] expectedValues)
        {
            var table = input.AsTable();
            AssertColumnNames(table, expectedColumns);
            AssertValues(table, input[index], expectedValues);
        }

        private static void AssertValues<T>(Table<T> table, T row, params ColumnValue[] expectedValues)
        {
            Assert.That(table.Columns.Select(c => c.GetValue(row)).ToArray(), Is.EqualTo(expectedValues));
        }

        private static void AssertColumnNames<T>(Table<T> table, params string[] expectedColumns)
        {
            Assert.That(table.Columns.Select(c => c.Name).ToArray(), Is.EqualTo(expectedColumns));
        }
    }
}
