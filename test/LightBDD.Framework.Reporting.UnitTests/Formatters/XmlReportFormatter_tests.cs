using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.Reporting.UnitTests.Formatters
{
    [TestFixture]
    public class XmlReportFormatter_tests
    {
        private IReportFormatter _subject;

        private static XmlSchemaSet _schema;

        public XmlReportFormatter_tests()
        {
            _schema = new XmlSchemaSet();
            _schema.Add("", AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "XmlReportFormatterSchema.xsd");
        }

        [SetUp]
        public void SetUp()
        {
            _subject = new XmlReportFormatter();
        }

        [Test]
        public void Should_format_xml()
        {
            var result = ReportFormatterTestData.GetFeatureResultWithDescription();
            var text = FormatResults(result);
            TestContext.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionEnd=""2014-09-23T19:23:00.155Z"" TestExecutionTime=""PT1M2.1S"">
    <Features Count=""1"" />
    <Scenarios Count=""2"" Passed=""0"" Bypassed=""0"" Failed=""1"" Ignored=""1"" />
    <Steps Count=""10"" Passed=""3"" Bypassed=""1"" Failed=""2"" Ignored=""2"" NotRun=""2"" />
  </Summary>
  <Feature Name=""My feature"">
    <Label Name=""Label 1"" />
    <Description>My feature
long description</Description>
    <Scenario Status=""Ignored"" Name=""name"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT1M2.1S"">
      <Name Format=""name"" />
      <Label Name=""Label 2"" />
      <Category Name=""categoryA"" />
      <Step Status=""Passed"" Number=""1"" Name=""call step1 &quot;arg1&quot;"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT1M1S"">
        <StepName StepType=""call"" Format=""step1 &quot;{0}&quot;"">
          <Parameter IsEvaluated=""true"">arg1</Parameter>
        </StepName>
        <Comment>multiline
comment</Comment>
        <Comment>comment 2</Comment>
      </Step>
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT1.1S"">
        <StatusDetails>Not implemented yet</StatusDetails>
        <StepName Format=""step2"" />
        <SubStep Status=""Passed"" Number=""1"" Name=""substep 1"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT0.1S"" GroupPrefix=""2."">
          <StepName Format=""substep 1"" />
        </SubStep>
        <SubStep Status=""Passed"" Number=""2"" Name=""substep 2"" ExecutionStart=""2014-09-23T19:22:00.155Z"" ExecutionTime=""PT1S"" GroupPrefix=""2."">
          <StepName Format=""substep 2"" />
        </SubStep>
        <SubStep Status=""Ignored"" Number=""3"" Name=""substep 3"" ExecutionStart=""2014-09-23T19:22:01.155Z"" ExecutionTime=""PT0S"" GroupPrefix=""2."">
          <StatusDetails>Not implemented yet</StatusDetails>
          <StepName Format=""substep 3"" />
          <Comment>sub-comment</Comment>
          <SubStep Status=""Failed"" Number=""1"" Name=""sub-substep 1"" GroupPrefix=""2.3."">
            <StepName Format=""sub-substep 1"" />
            <Parameter Name=""table1"">
              <Table Status=""NotApplicable"" Message=""tabular message"">
                <Column Index=""0"" Name=""Key"" IsKey=""true"" />
                <Column Index=""1"" Name=""X"" IsKey=""false"" />
                <Column Index=""2"" Name=""Y"" IsKey=""false"" />
                <Row Status=""Success"" Type=""Matching"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key1"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""NotApplicable"" Value=""1"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""2"" Status=""NotApplicable"" Value=""2"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                </Row>
                <Row Status=""Failure"" Type=""Matching"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key2"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""Failure"" Value=""1"" Expectation=""2"" Message=""value message"" />
                  <Value Index=""2"" Status=""NotApplicable"" Value=""4"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                </Row>
                <Row Status=""Failure"" Type=""Missing"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key3"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""Failure"" Value=""&lt;none&gt;"" Expectation=""3"" Message=""value message"" />
                  <Value Index=""2"" Status=""Failure"" Value=""&lt;none&gt;"" Expectation=""6"" Message=""value message"" />
                </Row>
                <Row Status=""Failure"" Type=""Surplus"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key4"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""Failure"" Value=""3"" Expectation=""&lt;none&gt;"" Message=""value message"" />
                  <Value Index=""2"" Status=""Failure"" Value=""6"" Expectation=""&lt;none&gt;"" Message=""value message"" />
                </Row>
              </Table>
            </Parameter>
            <Parameter Name=""table2"">
              <Table Status=""NotApplicable"" Message=""tabular message"">
                <Column Index=""0"" Name=""Key"" IsKey=""true"" />
                <Column Index=""1"" Name=""X"" IsKey=""false"" />
                <Column Index=""2"" Name=""Y"" IsKey=""false"" />
                <Row Status=""Success"" Type=""Matching"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key1"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""NotApplicable"" Value=""1"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""2"" Status=""NotApplicable"" Value=""2"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                </Row>
                <Row Status=""Failure"" Type=""Matching"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key2"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""Failure"" Value=""1"" Expectation=""2"" Message=""value message"" />
                  <Value Index=""2"" Status=""NotApplicable"" Value=""4"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                </Row>
                <Row Status=""Failure"" Type=""Missing"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key3"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""Failure"" Value=""&lt;none&gt;"" Expectation=""3"" Message=""value message"" />
                  <Value Index=""2"" Status=""Failure"" Value=""&lt;none&gt;"" Expectation=""6"" Message=""value message"" />
                </Row>
                <Row Status=""Failure"" Type=""Surplus"" Message=""row message"">
                  <Value Index=""0"" Status=""NotApplicable"" Value=""Key4"" Expectation=""&lt;null&gt;"" Message=""value message"" />
                  <Value Index=""1"" Status=""Failure"" Value=""3"" Expectation=""&lt;none&gt;"" Message=""value message"" />
                  <Value Index=""2"" Status=""Failure"" Value=""6"" Expectation=""&lt;none&gt;"" Message=""value message"" />
                </Row>
              </Table>
            </Parameter>
            <Parameter Name=""inline"">
              <Value Status=""NotApplicable"" Value=""foo"" Message=""inline message"" />
            </Parameter>
            <Comment>sub-sub-multiline
comment</Comment>
          </SubStep>
          <SubStep Status=""NotRun"" Number=""2"" Name=""sub-substep 2"" GroupPrefix=""2.3."">
            <StepName Format=""sub-substep 2"" />
          </SubStep>
        </SubStep>
      </Step>
      <StatusDetails>Step 2: Not implemented yet</StatusDetails>
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2 &quot;arg1&quot;"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT2.157S"">
      <Name Format=""name2 &quot;{0}&quot;"">
        <Parameter IsEvaluated=""true"">arg1</Parameter>
      </Name>
      <Category Name=""categoryB"" />
      <Category Name=""categoryC"" />
      <Step Status=""Bypassed"" Number=""1"" Name=""step3"" ExecutionStart=""2014-09-23T19:22:02.055Z"" ExecutionTime=""PT2.107S"">
        <StatusDetails>bypass reason</StatusDetails>
        <StepName Format=""step3"" />
      </Step>
      <Step Status=""Failed"" Number=""2"" Name=""step4"" ExecutionStart=""2014-09-23T19:22:03.055Z"" ExecutionTime=""PT0.05S"">
        <StatusDetails>  Expected: True
  But was: False</StatusDetails>
        <StepName Format=""step4"" />
      </Step>
      <Step Status=""NotRun"" Number=""3"" Name=""step5"">
        <StepName Format=""step5"" />
      </Step>
      <StatusDetails>Step 1: bypass reason
Step 2: Expected: True
	  But was: False</StatusDetails>
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
            ValidateWithSchema(text);
        }

        [Test]
        public void Should_format_xml_without_description_nor_label_nor_details()
        {
            var result = ReportFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();
            var text = FormatResults(result);
            TestContext.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionEnd=""2014-09-23T19:21:58.08Z"" TestExecutionTime=""PT0.025S"">
    <Features Count=""1"" />
    <Scenarios Count=""1"" Passed=""0"" Bypassed=""0"" Failed=""0"" Ignored=""1"" />
    <Steps Count=""2"" Passed=""1"" Bypassed=""0"" Failed=""0"" Ignored=""1"" NotRun=""0"" />
  </Summary>
  <Feature Name=""My feature"">
    <Scenario Status=""Ignored"" Name=""name"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT0.025S"">
      <Name Format=""name"" />
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.02S"">
        <StepName Format=""step1"" />
      </Step>
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT0.005S"">
        <StepName Format=""step2"" />
      </Step>
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
            ValidateWithSchema(text);
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var results = ReportFormatterTestData.GetMultipleFeatureResults();

            var text = FormatResults(results);
            TestContext.WriteLine(text);
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionEnd=""2014-09-23T19:22:01.075Z"" TestExecutionTime=""PT3.02S"">
    <Features Count=""2"" />
    <Scenarios Count=""2"" Passed=""2"" Bypassed=""0"" Failed=""0"" Ignored=""0"" />
    <Steps Count=""2"" Passed=""2"" Bypassed=""0"" Failed=""0"" Ignored=""0"" NotRun=""0"" />
  </Summary>
  <Feature Name=""My feature"">
    <Scenario Status=""Passed"" Name=""scenario1"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT0.02S"">
      <Name Format=""scenario1"" />
      <Category Name=""categoryA"" />
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.02S"">
        <StepName Format=""step1"" />
      </Step>
    </Scenario>
  </Feature>
  <Feature Name=""My feature2"">
    <Scenario Status=""Passed"" Name=""scenario1"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT0.02S"">
      <Name Format=""scenario1"" />
      <Category Name=""categoryB"" />
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:22:02.055Z"" ExecutionTime=""PT0.02S"">
        <StepName Format=""step1"" />
      </Step>
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
            ValidateWithSchema(text);
        }

        private static void ValidateWithSchema(string xml)
        {
            XDocument.Parse(xml).Validate(_schema, (o, e) => Assert.Fail(e.Message));
        }
        private string FormatResults(params IFeatureResult[] results)
        {
            using (var memory = new MemoryStream())
            {
                _subject.Format(memory, results);
                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }
    }
}