using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Parameters;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

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
            public new string Value { get; set; }
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

        class EmptyObject{}

        [Test]
        public void ToTable_should_infer_columns_from_class_collection()
        {
            TestCollectionToTable(new[]
                {
                    new Derived {Field = "a", Name = "b", Text = "c", Value = "d", Virtual = 5},
                    new Derived {Field = "a2", Name = "b2", Text = "c2", Value = "d2", Virtual = 10}
                },
                new[] { "Field", "Name", "Text", "Value", "Virtual" },
                1,
                new[] { ColumnValue.From("a2"), ColumnValue.From("b2"), ColumnValue.From("c2"), ColumnValue.From("d2"), ColumnValue.From(10) });
        }

        [Test]
        public void ToTable_should_infer_columns_from_struct_collection()
        {
            TestCollectionToTable(
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
        public void ToTable_should_infer_columns_from_ValueTuple_collection()
        {
            TestCollectionToTable(
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
        public void ToTable_should_infer_columns_from_unnamed_ValueTuple_collection()
        {
            TestCollectionToTable(
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
        public void ToTable_should_infer_columns_from_Tuple_collection()
        {
            TestCollectionToTable(
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
        public void ToTable_should_infer_columns_from_Int_collection()
        {
            TestCollectionToTable(
                new[] { 1, 2, 3 },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From(2) });
        }

        [Test]
        public void ToTable_should_infer_columns_from_String_collection()
        {
            TestCollectionToTable(
                new[] { "t1", "t2", "t3" },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From("t2") });
        }

        [Test]
        public void ToTable_should_infer_columns_from_Object_collection()
        {
            TestCollectionToTable(
                new object[] { "t1", 2, 'c' },
                new[] { "Item" },
                1,
                new[] { ColumnValue.From(2) });
        }

        [Test]
        public void ToTable_should_infer_columns_from_multi_dimensional_collection()
        {
            TestCollectionToTable(
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
        public void ToTable_should_infer_columns_from_ExpandoObject_collection()
        {
            var json = @"[
{""name"":""John""},
{""name"":""Sarah"",""surname"":""Smith""},
{""name"":""Joe"",""surname"":""Smith"",""age"":27}
]";
            TestCollectionToTable(
                JsonConvert.DeserializeObject<ExpandoObject[]>(json),
                new[] { "age", "name", "surname" },
                1,
                new[] { ColumnValue.None, ColumnValue.From("Sarah"), ColumnValue.From("Smith") });
        }

        [Test]
        public void ToTable_should_maintain_declaration_order_of_inferred_columns_for_complex_types_with_fields_being_first()
        {
            var data = new[]
            {
                new Derived()
            };

            var inputTable = data.ToTable(x => x.WithInferredColumns(InferredColumnsOrder.Declaration));
            AssertColumnNames(inputTable, "Field", "Value", "Text", "Virtual", "Name");
        }

        [Test]
        public void ToTable_should_maintain_declaration_order_of_inferred_columns_for_poco_types_with_fields_being_first()
        {
            var data = new[]
            {
                new Base()
            };

            var inputTable = data.ToTable(x => x.WithInferredColumns(InferredColumnsOrder.Declaration));
            AssertColumnNames(inputTable, "Field", "Name", "Value", "Virtual");
        }

        [Test]
        public void It_should_infer_columns_from_dynamic_collection_of_unified_item_types()
        {
            var values = new[]
                {
                    new Point(2, 3),
                    new Point(3, 4)
                }
                .Cast<dynamic>()
                .ToArray();

            TestCollectionToTable(
                values,
                new[] { "X", "Y" },
                1,
                new[] { ColumnValue.From(3), ColumnValue.From(4) });
        }

        [Test]
        public void ToTable_should_infer_columns_from_Dictionary()
        {
            var input = new Dictionary<string, Point>
            {
                {"key1", new Point(3, 5)},
                {"key2", new Point(2, 7)}
            };
            var table = input.ToTable();
            AssertColumnNames(table, "Key", "X", "Y");

            AssertValues(table, input.First(), ColumnValue.From("key1"), ColumnValue.From(3), ColumnValue.From(5));
        }

        [Test]
        public void ToTable_should_allow_defining_custom_columns()
        {
            var input = new[]
            {
                new {Id = 123, X = 5, Y = 7}
            };

            var table = input.ToTable(r => r
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

            var table = input.ToTable(r => r
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
            Assert.That(input.ToTable().AsEnumerable(), Is.EqualTo(input));
        }

        [Test]
        public void Table_should_allow_accessing_provided_rows_via_indexer()
        {
            var input = new[]
            {
                new {Id = 6, X = 2, Y = 5},
                new {Id = 7, X = 3, Y = 7}
            };
            var table = input.ToTable();
            Assert.That(table[0], Is.EqualTo(input[0]));
            Assert.That(table[1], Is.EqualTo(input[1]));
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Table_should_has_item_for_collection_of_empty_expando_objects(int count)
        {
            var input = Enumerable.Range(0, count).Select(_ => new ExpandoObject()).ToArray();
            var table = input.ToTable();
            AssertColumnNames(table, "Item");
            table.Count.ShouldBe(count);
            for (int i = 0; i < count; ++i)
                AssertValues(table, input[i], ColumnValue.From(input[i]));
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Table_should_has_item_column_for_collection_of_empty_objects(int count)
        {
            var input = Enumerable.Range(0, count).Select(_ => new EmptyObject()).ToArray();
            var table = input.ToTable();
            AssertColumnNames(table, "Item");
            table.Count.ShouldBe(count);
            for (int i = 0; i < count; ++i)
                AssertValues(table, input[i], ColumnValue.From(input[i]));
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Table_should_has_item_column_for_collection_of_empty_collections(int count)
        {
            var input = Enumerable.Range(0, count).Select(_ => Array.Empty<int>()).ToArray();
            var table = input.ToTable();
            AssertColumnNames(table, "Item");
            table.Count.ShouldBe(count);
            for (int i = 0; i < count; ++i)
                AssertValues(table, input[i], ColumnValue.From(input[i]));
        }

        private static void TestCollectionToTable<T>(T[] input, string[] expectedColumns, int index, ColumnValue[] expectedValues)
        {
            var table = input.ToTable();
            AssertColumnNames(table, expectedColumns);
            AssertValues(table, input[index], expectedValues);
        }

        private static void AssertValues<T>(InputTable<T> table, T row, params ColumnValue[] expectedValues)
        {
            Assert.That(table.Columns.Select(c => c.GetValue(row)).ToArray(), Is.EqualTo(expectedValues));
        }

        private static void AssertColumnNames<T>(InputTable<T> table, params string[] expectedColumns)
        {
            Assert.That(table.Columns.Select(c => c.Name).ToArray(), Is.EqualTo(expectedColumns));

            var details = (ITabularParameterDetails)((IComplexParameter)table).Details;
            Assert.That(details.Columns.Select(x => x.Name).ToArray(), Is.EqualTo(expectedColumns));
        }
    }
}
