using System;
using System.Xml.Linq;
using System.Xml.Schema;
using LightBDD.Results.Formatters;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
    [TestFixture]
    public class XmlResultFormatterTests
    {
        private IResultFormatter _subject;
        private static XmlSchemaSet _schema;

        #region Setup/Teardown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _schema = new XmlSchemaSet();
            _schema.Add("", "XmlResultFormatterSchema.xsd");
        }

        [SetUp]
        public void SetUp()
        {
            _subject = new XmlResultFormatter();
        }

        #endregion

        [Test]
        public void Should_format_xml()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithDescription();
            var text = _subject.Format(result);
            Console.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionTime=""PT1M4.257S"">
    <Features Count=""1"" />
    <Scenarios Count=""2"" Passed=""0"" Bypassed=""0"" Failed=""1"" Ignored=""1"" />
    <Steps Count=""5"" Passed=""1"" Bypassed=""1"" Failed=""1"" Ignored=""1"" NotRun=""1"" />
  </Summary>
  <Feature Name=""My feature"" Label=""Label 1"">
    <Description>My feature
long description</Description>
    <Scenario Status=""Ignored"" Name=""name"" Label=""Label 2"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT1M2.1S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT1M1S"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT1.1S"">
        <StatusDetails>Not implemented yet</StatusDetails>
      </Step>
      <StatusDetails>Step 2: Not implemented yet</StatusDetails>
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT2.157S"">
      <Step Status=""Bypassed"" Number=""1"" Name=""step3"" ExecutionStart=""2014-09-23T19:22:02.055Z"" ExecutionTime=""PT2.107S"">
        <StatusDetails>bypass reason</StatusDetails>
      </Step>
      <Step Status=""Failed"" Number=""2"" Name=""step4"" ExecutionStart=""2014-09-23T19:22:03.055Z"" ExecutionTime=""PT0.05S"">
        <StatusDetails>  Expected: True
  But was: False</StatusDetails>
      </Step>
      <Step Status=""NotRun"" Number=""3"" Name=""step5"" />
      <StatusDetails>Step 1: bypass reason
Step 2: Expected: True
	  But was: False</StatusDetails>
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
            ValidateWithSchema(text);
        }

        [Test]
        public void Should_format_xml_without_description_nor_label_nor_details()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();
            var text = _subject.Format(result);
            Console.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionTime=""PT0.025S"">
    <Features Count=""1"" />
    <Scenarios Count=""1"" Passed=""0"" Bypassed=""0"" Failed=""0"" Ignored=""1"" />
    <Steps Count=""2"" Passed=""1"" Bypassed=""0"" Failed=""0"" Ignored=""1"" NotRun=""0"" />
  </Summary>
  <Feature Name=""My feature"">
    <Scenario Status=""Ignored"" Name=""name"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT0.025S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.02S"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT0.005S"" />
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
            ValidateWithSchema(text);
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var results = ResultFormatterTestData.GetMultipleFeatureResults();

            var text = _subject.Format(results);
            Console.WriteLine(text);
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionTime=""PT0.04S"">
    <Features Count=""2"" />
    <Scenarios Count=""2"" Passed=""2"" Bypassed=""0"" Failed=""0"" Ignored=""0"" />
    <Steps Count=""2"" Passed=""2"" Bypassed=""0"" Failed=""0"" Ignored=""0"" NotRun=""0"" />
  </Summary>
  <Feature Name=""My feature"">
    <Scenario Status=""Passed"" Name=""scenario1"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT0.02S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.02S"" />
    </Scenario>
  </Feature>
  <Feature Name=""My feature2"">
    <Scenario Status=""Passed"" Name=""scenario1"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT0.02S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:22:02.055Z"" ExecutionTime=""PT0.02S"" />
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
            ValidateWithSchema(text);
        }

        private static void ValidateWithSchema(string xml)
        {
            XDocument.Parse(xml).Validate(_schema, (o, e) => Assert.Fail(e.Message));
        }
    }
}