using System.IO;
using System.Text;
using LightBDD.Core.Results;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.Reporting.UnitTests.Formatters
{
    [TestFixture]
    public class PlainTextReportFormatter_tests
    {
        private IReportFormatter _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new PlainTextReportFormatter();
        }

        #endregion

        [Test]
        public void Should_format_feature_with_description()
        {
            var result = ReportFormatterTestData.GetFeatureResultWithDescription();
            var text = FormatResults(result);
            TestContext.WriteLine(text);
            const string expectedText = @"Summary:
	Project             : Random.Tests
	Overall status      : Failed
	Execution start time: 2014-09-23 19:21:58 UTC
	Execution end time  : 2014-09-23 19:23:00 UTC
	Execution duration  : 1m 02s
	Number of features  : 1
	Number of scenarios : 2
	Passed scenarios    : 0
	Bypassed scenarios  : 0
	Failed scenarios    : 1
	Ignored scenarios   : 1
	Number of steps     : 10
	Passed steps        : 3
	Bypassed steps      : 1
	Failed steps        : 2
	Ignored steps       : 2
	Not Run steps       : 2
	LightBDD versions   : LightBDD.Framework v3.7.0.0, LightBDD.Core v3.7.0.0

Feature: My feature [Label 1]
	My feature
	long description

	Scenario: name [Label 2] - Ignored (1m 02s)
		Categories: categoryA
		Step 1: call step1 ""arg1"" - Passed (1m 01s)
		Step 2: step2 - Ignored (1s 100ms)
			Step 2.1: substep 1 - Passed (100ms)
			Step 2.2: substep 2 - Passed (1s)
			Step 2.3: substep 3 - Ignored (0ms)
				Step 2.3.1: sub-substep 1 - Failed
				table1:
				+-+----+--------+--------+
				|#|Key |X       |Y       |
				+-+----+--------+--------+
				|=|Key1|1       |2       |
				|!|Key2|1/2     |4       |
				|-|Key3|<none>/3|<none>/6|
				|+|Key4|3/<none>|6/<none>|
				+-+----+--------+--------+
				table2:
				+----+-+-+
				|Key |X|Y|
				+----+-+-+
				|Key1|1|2|
				|Key2|2|4|
				|Key3|3|6|
				+----+-+-+
				Step 2.3.2: sub-substep 2 - NotRun
		Details:
			Step 2: Not implemented yet
		Comments:
			Step 1: multiline
				comment
			Step 1: comment 2
			Step 2.3: sub-comment
			Step 2.3.1: sub-sub-multiline
				comment
		Attachments:
			Step 2.3.1: attachment1 - file1.png

	Scenario: name2 ""arg1"" - Failed (2s 157ms)
		Categories: categoryB, categoryC
		Step 1: step3 - Bypassed (2s 107ms)
		Step 2: step4 - Failed (50ms)
		Step 3: step5 - NotRun
		Details:
			Step 1: bypass reason
			Step 2: Expected: True
				  But was: False
";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_feature_without_description_nor_label_nor_details()
        {
            var result = ReportFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();
            var text = FormatResults(result);
            TestContext.WriteLine(text);
            const string expectedText = @"Summary:
	Project             : Random.Tests
	Overall status      : Passed
	Execution start time: 2014-09-23 19:21:58 UTC
	Execution end time  : 2014-09-23 19:21:58 UTC
	Execution duration  : 25ms
	Number of features  : 1
	Number of scenarios : 1
	Passed scenarios    : 0
	Bypassed scenarios  : 0
	Failed scenarios    : 0
	Ignored scenarios   : 1
	Number of steps     : 2
	Passed steps        : 1
	Bypassed steps      : 0
	Failed steps        : 0
	Ignored steps       : 1
	Not Run steps       : 0
	LightBDD versions   : LightBDD.Framework v3.7.0.0, LightBDD.Core v3.7.0.0

Feature: My feature

	Scenario: name - Ignored (25ms)
		Step 1: step1 - Passed (20ms)
		Step 2: step2 - Ignored (5ms)
";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var results = ReportFormatterTestData.GetMultipleFeatureResults();

            var text = FormatResults(results);
            TestContext.WriteLine(text);
            const string expectedText = @"Summary:
	Project             : Random.Tests
	Overall status      : Passed
	Execution start time: 2014-09-23 19:21:58 UTC
	Execution end time  : 2014-09-23 19:22:01 UTC
	Execution duration  : 3s 020ms
	Number of features  : 2
	Number of scenarios : 2
	Passed scenarios    : 2
	Bypassed scenarios  : 0
	Failed scenarios    : 0
	Ignored scenarios   : 0
	Number of steps     : 2
	Passed steps        : 2
	Bypassed steps      : 0
	Failed steps        : 0
	Ignored steps       : 0
	Not Run steps       : 0
	LightBDD versions   : LightBDD.Framework v3.7.0.0, LightBDD.Core v3.7.0.0

Feature: My feature

	Scenario: scenario1 - Passed (20ms)
		Categories: categoryA
		Step 1: step1 - Passed (20ms)

Feature: My feature2

	Scenario: scenario1 - Passed (20ms)
		Categories: categoryB
		Step 1: step1 - Passed (20ms)
";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_scenarios_in_order()
        {
            var results = ReportFormatterTestData.GetFeatureWithUnsortedScenarios();

            var text = FormatResults(results);
            TestContext.WriteLine(text);
            const string expectedText = @"Summary:
	Project             : Random.Tests
	Overall status      : Passed
	Execution start time: 2014-09-23 19:21:57 UTC
	Execution end time  : 2014-09-23 19:22:02 UTC
	Execution duration  : 5s
	Number of features  : 1
	Number of scenarios : 3
	Passed scenarios    : 3
	Bypassed scenarios  : 0
	Failed scenarios    : 0
	Ignored scenarios   : 0
	Number of steps     : 3
	Passed steps        : 3
	Bypassed steps      : 0
	Failed steps        : 0
	Ignored steps       : 0
	Not Run steps       : 0
	LightBDD versions   : LightBDD.Framework v3.7.0.0, LightBDD.Core v3.7.0.0

Feature: My Feature

	Scenario: scenario A [lab B] - Passed (2s)
		Step 1: step - Passed

	Scenario: scenario B [lab C] - Passed (5s)
		Step 1: step - Passed

	Scenario: scenario C [lab A] - Passed (2s)
		Step 1: step - Passed
";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_verifiable_trees()
        {
            var expected = new
            {
                Name = "John",
                Surname = "Johnson",
                Address = new { Street = "High Street", PostCode = "AB1 7BA", City = "London", Country = "UK" },
                Records = new[] { "AB-1", "AB-2", "AB-3" }
            };
            var actual = new
            {
                Name = "Johnny",
                Surname = "Johnson",
                Address = new { Street = "High Street", PostCode = "AB1 7BC", City = "London", Country = "UK" },
                Records = new[] { "AB-1", "AB-2", "AB-3", "AB-4" }
            };

            var tree = Tree.ExpectEquivalent(expected);
            tree.SetActual(actual);

            var results = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
             var text = FormatResults(results);
            TestContext.WriteLine(text);
            const string expectedText = @"Summary:
	Project             : Random.Tests
	Overall status      : Failed
	Execution start time: 2014-09-23 19:21:57 UTC
	Execution end time  : 2014-09-23 19:21:59 UTC
	Execution duration  : 2s
	Number of features  : 1
	Number of scenarios : 1
	Passed scenarios    : 0
	Bypassed scenarios  : 0
	Failed scenarios    : 1
	Ignored scenarios   : 0
	Number of steps     : 1
	Passed steps        : 0
	Bypassed steps      : 0
	Failed steps        : 1
	Ignored steps       : 0
	Not Run steps       : 0
	LightBDD versions   : LightBDD.Framework v3.7.0.0, LightBDD.Core v3.7.0.0

Feature: My Feature

	Scenario: scenario A [lab B] - Failed (2s)
		Step 1: step - Failed
		tree:
		= $: <object>
		= $.Address: <object>
		= $.Address.City: London
		= $.Address.Country: UK
		! $.Address.PostCode: AB1 7BA/AB1 7BC
		= $.Address.Street: High Street
		! $.Name: John/Johnny
		! $.Records: <array:3>/<array:4>
		= $.Records[0]: AB-1
		= $.Records[1]: AB-2
		= $.Records[2]: AB-3
		! $.Records[3]: <none>/AB-4
		= $.Surname: Johnson
";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        private string FormatResults(ITestRunResult result)
        {
            using (var memory = new MemoryStream())
            {
                _subject.Format(memory, result);
                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }
    }
}
