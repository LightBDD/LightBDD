using System;
using LightBDD.Results.Formatters;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
    [TestFixture]
    public class PlainTextResultFormatterTests
    {
        private IResultFormatter _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new PlainTextResultFormatter();
        }

        #endregion

        [Test]
        public void Should_format_feature_with_description()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithDescription();
            var text = _subject.Format(result);
            Console.WriteLine(text);
            const string expectedText = @"Summary:
	Test execution start time: 2014-09-23 19:21:58 UTC
	Test execution time      : 1m 04s
	Number of features       : 1
	Number of scenarios      : 2
	Passed scenarios         : 0
	Bypassed scenarios       : 0
	Failed scenarios         : 1
	Ignored scenarios        : 1
	Number of steps          : 5
	Passed steps             : 1
	Bypassed steps           : 1
	Failed steps             : 1
	Ignored steps            : 1
	Not Run steps            : 1

Feature: My feature [Label 1]
	My feature
	long description

	Scenario: name [Label 2] - Ignored (1m 02s)
		Step 1: step1 - Passed (1m 01s)
		Step 2: step2 - Ignored (1s 100ms)
			Details: Not implemented yet

		Details: Not implemented yet

	Scenario: name2 - Failed (2s 157ms)
		Step 1: step3 - Bypassed (2s 107ms)
			Details: bypass reason
		Step 2: step4 - Failed (50ms)
			Details: Expected: True
				  But was: False
		Step 3: step5 - NotRun

		Details: Expected: True
			  But was: False
";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_feature_without_description_nor_label_nor_details()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();
            var text = _subject.Format(result);
            Console.WriteLine(text);
            const string expectedText = @"Summary:
	Test execution start time: 2014-09-23 19:21:58 UTC
	Test execution time      : 25ms
	Number of features       : 1
	Number of scenarios      : 1
	Passed scenarios         : 0
	Bypassed scenarios       : 0
	Failed scenarios         : 0
	Ignored scenarios        : 1
	Number of steps          : 2
	Passed steps             : 1
	Bypassed steps           : 0
	Failed steps             : 0
	Ignored steps            : 1
	Not Run steps            : 0

Feature: My feature

	Scenario: name - Ignored (25ms)
		Step 1: step1 - Passed (20ms)
		Step 2: step2 - Ignored (5ms)
";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var results = ResultFormatterTestData.GetMultipleFeatureResults();

            var text = _subject.Format(results);
            Console.WriteLine(text);
            const string expectedText = @"Summary:
	Test execution start time: 2014-09-23 19:21:58 UTC
	Test execution time      : 40ms
	Number of features       : 2
	Number of scenarios      : 2
	Passed scenarios         : 2
	Bypassed scenarios       : 0
	Failed scenarios         : 0
	Ignored scenarios        : 0
	Number of steps          : 2
	Passed steps             : 2
	Bypassed steps           : 0
	Failed steps             : 0
	Ignored steps            : 0
	Not Run steps            : 0

Feature: My feature

	Scenario: scenario1 - Passed (20ms)
		Step 1: step1 - Passed (20ms)

Feature: My feature2

	Scenario: scenario1 - Passed (20ms)
		Step 1: step1 - Passed (20ms)
";
            Assert.That(text, Is.EqualTo(expectedText));
        }
    }
}
