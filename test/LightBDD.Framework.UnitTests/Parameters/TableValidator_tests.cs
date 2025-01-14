﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using NUnit.Framework;
#pragma warning disable 1998

namespace LightBDD.Framework.UnitTests.Parameters
{
    [TestFixture]
    public class TableValidator_tests
    {
        class Testable
        {
            public Testable(string name, string value)
            {
                Name = name;
                Value = value;
            }
            public string Name { get; }
            public string Value { get; }
        }


        private static TableValidator<Testable> CreateNotNullValidator()
        {
            return Table.Validate<Testable>(b => b
                .WithColumn(x => x.Name, Expect.To.Not.BeNull())
                .WithColumn(x => x.Value, Expect.To.Not.BeNull())
            );
        }

        [Test]
        public void TableValidator_with_no_actual_should_have_NotProvided_status()
        {
            var validator = CreateNotNullValidator();
            AssertResultColumnsMatchingTable(validator);
            var result = validator.Details;

            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.NotProvided));
            Assert.That(result.Rows.Count, Is.EqualTo(1));

            AssertRow(result.Rows[0], TableRowType.Missing, ParameterVerificationStatus.Failure, "NotProvided|<none>|not null|null", "NotProvided|<none>|not null|null");
            AssertVerificationMessage(result.Rows[0].VerificationMessage, null);
            AssertVerificationMessage(result.VerificationMessage, null);
        }

        [Test]
        public void SetActual_should_set_actual_values()
        {
            var input = new[]
            {
                new Testable("name","value"),
                new Testable("name2","value2")
            };

            var table = CreateNotNullValidator();
            table.SetActual(input);
            Assert.That(table.ActualRows, Is.EqualTo(input));
            Assert.That(table.Details.VerificationStatus, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
        }

        [Test]
        public async Task SetActualAsync_should_set_actual_values()
        {
            var input = new[]
            {
                new Testable("name","value"),
                new Testable("name2","value2")
            };

            var table = CreateNotNullValidator();
            await table.SetActualAsync(async () => input);
            Assert.That(table.ActualRows, Is.EqualTo(input));
            Assert.That(table.Details.VerificationStatus, Is.Not.EqualTo(ParameterVerificationStatus.NotProvided));
        }

        [Test]
        public void SetActual_should_be_callable_once()
        {
            var table = CreateNotNullValidator();
            table.SetActual(Array.Empty<Testable>());

            var ex = Assert.Throws<InvalidOperationException>(() => table.SetActual(Array.Empty<Testable>()));
            Assert.That(ex.Message, Is.EqualTo("Actual rows have been already specified"));
            Assert.ThrowsAsync<InvalidOperationException>(() => table.SetActualAsync(() => Task.FromResult(Enumerable.Empty<Testable>())));
        }

        [Test]
        public void ResetActual_should_reset_table_and_allow_setting_another_value()
        {
            var table = CreateNotNullValidator();
            table.SetActual(Array.Empty<Testable>());
            table.ResetActual();
            Assert.That(table.Details.VerificationStatus,Is.EqualTo(ParameterVerificationStatus.NotProvided));

            Assert.DoesNotThrow(() => table.SetActual(Array.Empty<Testable>()));
            Assert.That(table.Details.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Success));
        }

        [Test]
        public async Task SetActualAsync_should_capture_exception()
        {
            var table = CreateNotNullValidator();
            await table.SetActualAsync(() => throw new Exception("foo"));
            Assert.That(table.Details.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Exception));
            Assert.That(table.Details.VerificationMessage, Is.EqualTo("Failed to retrieve rows: foo"));
            Assert.That(table.ActualRows, Is.Empty);
        }

        [Test]
        public async Task SetActualAsync_should_capture_exception_and_still_should_be_callable_once()
        {
            var table = CreateNotNullValidator();
            await table.SetActualAsync(() => throw new Exception("foo"));

            var ex = Assert.Throws<InvalidOperationException>(() => table.SetActual(Enumerable.Empty<Testable>()));
            Assert.That(ex.Message, Is.EqualTo("Actual rows have been already specified"));
        }

        [Test]
        public void SetActual_should_not_accept_nulls()
        {
            var table = CreateNotNullValidator();
            Assert.Throws<ArgumentNullException>(() => table.SetActual(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => table.SetActualAsync(null));
        }

        [Test]
        public void SetActual_should_evaluate_expectations_for_each_row()
        {
            var table = CreateNotNullValidator();
            table.SetActual(new[]
            {
                new Testable("name", "value"),
                new Testable(null, null),
                new Testable(null, "value3"),
                new Testable("name4", null)
            });
            var result = table.Details;
            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Failure));
            Assert.That(result.Rows.Count, Is.EqualTo(4));

            AssertRow(result.Rows[0], TableRowType.Matching, ParameterVerificationStatus.Success, "Success|name|not null|null", "Success|value|not null|null");
            AssertRow(result.Rows[1], TableRowType.Matching, ParameterVerificationStatus.Failure, "Failure|<null>|not null|Name: expected: not null, but it was", "Failure|<null>|not null|Value: expected: not null, but it was");
            AssertRow(result.Rows[2], TableRowType.Matching, ParameterVerificationStatus.Failure, "Failure|<null>|not null|Name: expected: not null, but it was", "Success|value3|not null|null");
            AssertRow(result.Rows[3], TableRowType.Matching, ParameterVerificationStatus.Failure, "Success|name4|not null|null", "Failure|<null>|not null|Value: expected: not null, but it was");
            AssertVerificationMessage(result.Rows[1].VerificationMessage, "[1].Name: expected: not null, but it was\n[1].Value: expected: not null, but it was");
            AssertVerificationMessage(result.Rows[2].VerificationMessage, "[2].Name: expected: not null, but it was");
            AssertVerificationMessage(result.Rows[3].VerificationMessage, "[3].Value: expected: not null, but it was");
            AssertVerificationMessage(result.VerificationMessage, "[1].Name: expected: not null, but it was\n[1].Value: expected: not null, but it was\n[2].Name: expected: not null, but it was\n[3].Value: expected: not null, but it was");
        }

        [Test]
        public async Task SetActualAsync_should_evaluate_expectations_for_each_row()
        {
            var table = CreateNotNullValidator();
            await table.SetActualAsync(async () => new[]
            {
                new Testable("name", "value"),
                new Testable(null, null),
                new Testable(null, "value3"),
                new Testable("name4", null)
            });
            var result = table.Details;
            Assert.That(result.VerificationStatus, Is.EqualTo(ParameterVerificationStatus.Failure));
            Assert.That(result.Rows.Count, Is.EqualTo(4));

            AssertRow(result.Rows[0], TableRowType.Matching, ParameterVerificationStatus.Success, "Success|name|not null|null", "Success|value|not null|null");
            AssertRow(result.Rows[1], TableRowType.Matching, ParameterVerificationStatus.Failure, "Failure|<null>|not null|Name: expected: not null, but it was", "Failure|<null>|not null|Value: expected: not null, but it was");
            AssertRow(result.Rows[2], TableRowType.Matching, ParameterVerificationStatus.Failure, "Failure|<null>|not null|Name: expected: not null, but it was", "Success|value3|not null|null");
            AssertRow(result.Rows[3], TableRowType.Matching, ParameterVerificationStatus.Failure, "Success|name4|not null|null", "Failure|<null>|not null|Value: expected: not null, but it was");
            AssertVerificationMessage(result.Rows[1].VerificationMessage, "[1].Name: expected: not null, but it was\n[1].Value: expected: not null, but it was");
            AssertVerificationMessage(result.Rows[2].VerificationMessage, "[2].Name: expected: not null, but it was");
            AssertVerificationMessage(result.Rows[3].VerificationMessage, "[3].Value: expected: not null, but it was");
            AssertVerificationMessage(result.VerificationMessage, "[1].Name: expected: not null, but it was\n[1].Value: expected: not null, but it was\n[2].Name: expected: not null, but it was\n[3].Value: expected: not null, but it was");
        }

        private void AssertRow(ITabularParameterRow row, TableRowType rowType, ParameterVerificationStatus rowStatus, params string[] expectedValueDetails)
        {
            Assert.That(row.Type, Is.EqualTo(rowType));

            var actual = row.Values
                .Select(v => $"{v.VerificationStatus}|{v.Value}|{v.Expectation}|{v.VerificationMessage ?? "null"}")
                .ToArray();
            Assert.That(actual, Is.EqualTo(expectedValueDetails));

            Assert.That(row.VerificationStatus, Is.EqualTo(rowStatus));
        }

        private static void AssertResultColumnsMatchingTable<T>(VerifiableTable<T> table)
        {
            Assert.That(table.Details.Columns.Select(c => $"{c.IsKey}|{c.Name}").ToArray(),
                Is.EqualTo(table.Columns.Select(c => $"{c.IsKey}|{c.Name}").ToArray()));
        }

        private static void AssertVerificationMessage(string actual, string expected)
        {
            Assert.That(actual, Is.EqualTo(expected?.Replace("\n", Environment.NewLine)));
        }
    }
}