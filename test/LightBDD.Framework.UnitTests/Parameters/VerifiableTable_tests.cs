using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
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
                new {Id = 123, X = 5, Y = 7}
            };

            var table = input.AsVerifiableTable(r => r
                .WithKey(v => v.Id)
                .WithKey("Secondary", v => $"{v.Id}_{v.X}_{v.Y}")
                .WithColumn(v => v.X)
                .WithColumn(v => v.Y)
                .WithColumn("Sum", v => v.X + v.Y));

            AssertColumnNames(table, "Id", "Secondary", "X", "Y", "Sum");
            Assert.That(table.Columns[0].IsKey, Is.True);
            Assert.That(table.Columns[1].IsKey, Is.True);
            Assert.That(table.Columns[2].IsKey, Is.False);
            Assert.That(table.Columns[3].IsKey, Is.False);
            Assert.That(table.Columns[4].IsKey, Is.False);

            AssertValues(table, input[0], ColumnValue.From(123), ColumnValue.From("123_5_7"), ColumnValue.From(5), ColumnValue.From(7), ColumnValue.From(12));
        }

        [Test]
        public void Expected_should_return_provided_rows()
        {
            var input = new[]
            {
                new {Id = 6, X = 2, Y = 5},
                new {Id = 7, X = 3, Y = 7}
            };
            Assert.That(input.AsVerifiableTable().Expected, Is.EqualTo(input));
        }

        [Test]
        public void VerifiableTable_with_no_actual_should_have_NotProvided_status()
        {
            var input = new[] { "abc", "def" };
            var table = input.AsVerifiableTable();
            AssertResultColumnsMatchingTable(table);
            var result = table.Result;

            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.NotProvided));
            Assert.That(result.Rows.Count, Is.EqualTo(2));

            AssertRow(result.Rows[0], TableRowType.Missing, ParameterVerificationStatus.Failure, "NotProvided|<none>|abc|Item: Value not provided");
            AssertRow(result.Rows[1], TableRowType.Missing, ParameterVerificationStatus.Failure, "NotProvided|<none>|def|Item: Value not provided");
            Assert.That(result.Rows[0].Exception?.Message, Is.EqualTo("[0].Item: Value not provided"));
            Assert.That(result.Rows[1].Exception?.Message, Is.EqualTo("[1].Item: Value not provided"));
            Assert.That(result.Exception?.Message, Is.EqualTo("[0].Item: Value not provided\n[1].Item: Value not provided"));
        }

        [Test]
        public void SetActual_should_set_actual_values()
        {
            var input = new[]
            {
                new {Id = 6, X = 2, Y = 5},
                new {Id = 7, X = 3, Y = 7}
            };

            var table = input.AsVerifiableTable().SetActual(input);
            Assert.That(table.Actual, Is.EqualTo(input));
            Assert.That(table.Result.VerificationStatus, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
        }

        [Test]
        public async Task SetActualAsync_should_set_actual_values()
        {
            var input = new[]
            {
                new {Id = 6, X = 2, Y = 5},
                new {Id = 7, X = 3, Y = 7}
            };

            var table = await input.AsVerifiableTable().SetActualAsync(async () => input);
            Assert.That(table.Actual, Is.EqualTo(input));
            Assert.That(table.Result.VerificationStatus, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
        }

        [Test]
        public void SetActual_with_lookup_should_set_actual_values()
        {
            var input = new[]
                {
                    new {Id = 6, X = 2, Y = 5},
                    new {Id = 7, X = 3, Y = 7}
                };

            var table = input.AsVerifiableTable().SetActual(expected => expected);
            Assert.That(table.Actual, Is.EqualTo(input));
            Assert.That(table.Result.VerificationStatus, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
        }

        [Test]
        public async Task SetActualAsync_with_lookup_should_set_actual_values()
        {
            var input = new[]
            {
                new {Id = 6, X = 2, Y = 5},
                new {Id = 7, X = 3, Y = 7}
            };

            var table = await input.AsVerifiableTable().SetActualAsync(async expected => expected);
            Assert.That(table.Actual, Is.EqualTo(input));
            Assert.That(table.Result.VerificationStatus, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
        }

        [Test]
        public void SetActual_should_use_row_index_for_matching_if_no_key_columns_were_defined()
        {
            var expected = new[]
            {
                new {X = 1, Y = 2},
                new {X = 2, Y = 4},
                new {X = 3, Y = 6},
                new {X = 4, Y = 8}
            };
            var actual = new[]
            {
                new {X = 1, Y = 2},
                new {X = 2, Y = 2},
                new {X = -3, Y = -6},
                new {X = 4, Y = 8}
            };

            var table = expected.AsVerifiableTable().SetActual(actual);
            AssertResultColumnsMatchingTable(table);

            var result = table.Result;
            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Failure));
            Assert.That(result.Rows.Count, Is.EqualTo(4));

            AssertRow(result.Rows[0], TableRowType.Matching, ParameterVerificationStatus.Success, "Success|1|1|null", "Success|2|2|null");
            AssertRow(result.Rows[1], TableRowType.Matching, ParameterVerificationStatus.Failure, "Success|2|2|null", "Failure|2|4|Y: expected: equal '4', but got: '2'");
            AssertRow(result.Rows[2], TableRowType.Matching, ParameterVerificationStatus.Failure, "Failure|-3|3|X: expected: equal '3', but got: '-3'", "Failure|-6|6|Y: expected: equal '6', but got: '-6'");
            AssertRow(result.Rows[3], TableRowType.Matching, ParameterVerificationStatus.Success, "Success|4|4|null", "Success|8|8|null");
            Assert.That(result.Rows[1].Exception?.Message, Is.EqualTo("[1].Y: expected: equal '4', but got: '2'"));
            Assert.That(result.Rows[2].Exception?.Message, Is.EqualTo("[2].X: expected: equal '3', but got: '-3'\n[2].Y: expected: equal '6', but got: '-6'"));
            Assert.That(result.Exception?.Message, Is.EqualTo("[1].Y: expected: equal '4', but got: '2'\n[2].X: expected: equal '3', but got: '-3'\n[2].Y: expected: equal '6', but got: '-6'"));
        }

        [Test]
        public void SetActual_with_row_index_matching_should_report_surplus_rows()
        {
            var expected = new[]
            {
                new {X = 1, Y = 2},
            };
            var actual = new[]
            {
                new {X = 1, Y = 2},
                new {X = 2, Y = 2},
                new {X = 3, Y = 6}
            };

            var table = expected.AsVerifiableTable().SetActual(actual);
            AssertResultColumnsMatchingTable(table);

            var result = table.Result;
            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Failure));
            Assert.That(result.Rows.Count, Is.EqualTo(3));

            AssertRow(result.Rows[0], TableRowType.Matching, ParameterVerificationStatus.Success, "Success|1|1|null", "Success|2|2|null");
            AssertRow(result.Rows[1], TableRowType.Surplus, ParameterVerificationStatus.Failure, "Failure|2|<none>|X: unexpected value", "Failure|2|<none>|Y: unexpected value");
            AssertRow(result.Rows[2], TableRowType.Surplus, ParameterVerificationStatus.Failure, "Failure|3|<none>|X: unexpected value", "Failure|6|<none>|Y: unexpected value");
        }

        [Test]
        public void SetActual_with_row_index_matching_should_report_missing_rows()
        {
            var expected = new[]
            {
                new {X = 1, Y = 2},
                new {X = 2, Y = 2},
                new {X = 3, Y = 6}
            };
            var actual = new[]
            {
                new {X = 1, Y = 2},
            };

            var table = expected.AsVerifiableTable().SetActual(actual);
            AssertResultColumnsMatchingTable(table);

            var result = table.Result;
            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Failure));
            Assert.That(result.Rows.Count, Is.EqualTo(3));

            AssertRow(result.Rows[0], TableRowType.Matching, ParameterVerificationStatus.Success, "Success|1|1|null", "Success|2|2|null");
            AssertRow(result.Rows[1], TableRowType.Missing, ParameterVerificationStatus.Failure, "Failure|<none>|2|X: missing value", "Failure|<none>|2|Y: missing value");
            AssertRow(result.Rows[2], TableRowType.Missing, ParameterVerificationStatus.Failure, "Failure|<none>|3|X: missing value", "Failure|<none>|6|Y: missing value");
        }

        [Test]
        public void SetActual_with_key_columns_should_match_record_by_keys_then_row_index()
        {
            var expected = new[]
            {
                new {Key1 = "A", Key2 = 1, Key3 = 0, Value = 100},
                new {Key1 = "A", Key2 = 1, Key3 = 0, Value = 101},
                new {Key1 = "A", Key2 = 1, Key3 = 0, Value = 102},
                new {Key1 = "A", Key2 = 1, Key3 = 1, Value = 103},
                new {Key1 = "A", Key2 = 2, Key3 = 0, Value = 104},
                new {Key1 = "B", Key2 = 0, Key3 = 0, Value = 105},
                new {Key1 = "B", Key2 = 0, Key3 = 0, Value = 106},
                new {Key1 = "D", Key2 = 0, Key3 = 0, Value = 107}
            };

            var actual = new[]
            {
                new {Key1 = "A", Key2 = 1, Key3 = 0, Value = 100},
                new {Key1 = "A", Key2 = 1, Key3 = 0, Value = 101},
                new {Key1 = "C", Key2 = 0, Key3 = 0, Value = 1000},
                new {Key1 = "A", Key2 = 2, Key3 = 0, Value = 104},
                new {Key1 = "A", Key2 = 1, Key3 = 1, Value = 103},
                new {Key1 = "B", Key2 = 0, Key3 = 0, Value = 106},
                new {Key1 = "B", Key2 = 0, Key3 = 0, Value = 105}
            };

            var table = expected.AsVerifiableTable(x => x
                    .WithKey(c => c.Key1)
                    .WithKey(c => c.Key2)
                    .WithKey(c => c.Key3)
                    .WithColumn(c => c.Value))
                .SetActual(actual);
            AssertResultColumnsMatchingTable(table);

            var result = table.Result;
            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Failure));
            Assert.That(result.Rows.Count, Is.EqualTo(9));

            AssertRowValues(result.Rows[0], TableRowType.Matching, ParameterVerificationStatus.Success, "A|A", "1|1", "0|0", "100|100");
            AssertRowValues(result.Rows[1], TableRowType.Matching, ParameterVerificationStatus.Success, "A|A", "1|1", "0|0", "101|101");
            AssertRowValues(result.Rows[2], TableRowType.Missing, ParameterVerificationStatus.Failure, "<none>|A", "<none>|1", "<none>|0", "<none>|102");
            AssertRowValues(result.Rows[3], TableRowType.Matching, ParameterVerificationStatus.Success, "A|A", "1|1", "1|1", "103|103");
            AssertRowValues(result.Rows[4], TableRowType.Matching, ParameterVerificationStatus.Success, "A|A", "2|2", "0|0", "104|104");
            AssertRowValues(result.Rows[5], TableRowType.Matching, ParameterVerificationStatus.Failure, "B|B", "0|0", "0|0", "106|105");
            AssertRowValues(result.Rows[6], TableRowType.Matching, ParameterVerificationStatus.Failure, "B|B", "0|0", "0|0", "105|106");
            AssertRowValues(result.Rows[7], TableRowType.Missing, ParameterVerificationStatus.Failure, "<none>|D", "<none>|0", "<none>|0", "<none>|107");
            AssertRowValues(result.Rows[8], TableRowType.Surplus, ParameterVerificationStatus.Failure, "C|<none>", "0|<none>", "0|<none>", "1000|<none>");
        }

        private void AssertRow(ITabularParameterRow row, TableRowType rowType, ParameterVerificationStatus rowStatus, params string[] expectedValueDetails)
        {
            Assert.That(row.Type, Is.EqualTo(rowType));
            Assert.That(row.VerificationStatus, Is.EqualTo(rowStatus));

            var actual = row.Values
                .Select(v => $"{v.VerificationStatus}|{v.Value}|{v.Expectation}|{v.Exception?.Message ?? "null"}")
                .ToArray();
            Assert.That(actual, Is.EqualTo(expectedValueDetails));
        }

        private void AssertRowValues(ITabularParameterRow row, TableRowType rowType, ParameterVerificationStatus rowStatus, params string[] expectedValues)
        {
            Assert.That(row.Type, Is.EqualTo(rowType));
            Assert.That(row.VerificationStatus, Is.EqualTo(rowStatus));

            var actual = row.Values
                .Select(v => $"{v.Value}|{v.Expectation}")
                .ToArray();
            Assert.That(actual, Is.EqualTo(expectedValues));
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

        private static void AssertResultColumnsMatchingTable<T>(VerifiableTable<T> table)
        {
            Assert.That(table.Result.Columns.Select(c => $"{c.IsKey}|{c.Name}").ToArray(),
                Is.EqualTo(table.Columns.Select(c => $"{c.IsKey}|{c.Name}").ToArray()));
        }
    }
}