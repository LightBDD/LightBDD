using System;
using NUnit.Framework;
using SimpleBDD.Results;
using SimpleBDD.Results.Formatters;
using SimpleBDD.Results.Implementation;

namespace SimpleBDD.UnitTests.Results.Formatters
{
	[TestFixture]
	public class XmlResultFormatterTests
	{
		private IResultFormatter _subject;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new XmlResultFormatter();
		}

		#endregion

		[Test]
		public void Should_format_xml()
		{
			var result = new FeatureResult("Feature name", "feature\nlong description");
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, "step1", ResultStatus.Passed), new StepResult(2, "step2", ResultStatus.Ignored) }));
			result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, "step3", ResultStatus.Passed), new StepResult(2, "step4", ResultStatus.Failed) }));
			var text = _subject.Format(result);
			Console.WriteLine(text);

			const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Feature Name=""Feature name"">
    <Description>feature
long description</Description>
    <Scenario Status=""Ignored"" Name=""name"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" />
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"">
      <Step Status=""Passed"" Number=""1"" Name=""step3"" />
      <Step Status=""Failed"" Number=""2"" Name=""step4"" />
    </Scenario>
  </Feature>
</TestResults>";
			Assert.That(text, Is.EqualTo(expectedText));
		}
	}
}