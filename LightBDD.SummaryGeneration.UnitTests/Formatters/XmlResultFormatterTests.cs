using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using LightBDD.Core.Execution.Results;
using LightBDD.SummaryGeneration.Formatters;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.SummaryGeneration.UnitTests.Formatters
{
    [TestFixture]
    public class XmlResultFormatterTests
    {
        private IResultFormatter _subject;
        private static XmlSchemaSet _schema;

        #region Setup/Teardown

        public XmlResultFormatterTests()
        {
            _schema = new XmlSchemaSet();

            _schema.Add("", Path.GetDirectoryName(typeof(ISummaryWriter).Assembly.CodeBase) + "\\..\\..\\..\\..\\..\\XmlResultFormatterSchema.xsd");
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
            var text = FormatResults(result);
            TestContext.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionEnd=""2014-09-23T19:23:00.155Z"" TestExecutionTime=""PT1M2.1S"">
    <Features Count=""1"" />
    <Scenarios Count=""2"" Passed=""0"" Bypassed=""0"" Failed=""1"" Ignored=""1"" />
    <Steps Count=""5"" Passed=""1"" Bypassed=""1"" Failed=""1"" Ignored=""1"" NotRun=""1"" />
  </Summary>
  <Feature Name=""My feature"">
    <Label Name=""Label 1"" />
    <Description>My feature
long description</Description>
    <Scenario Status=""Ignored"" Name=""name"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT1M2.1S"">
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
      </Step>
      <StatusDetails>Step 2: Not implemented yet</StatusDetails>
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT2.157S"">
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
            var result = ResultFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();
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
            var results = ResultFormatterTestData.GetMultipleFeatureResults();

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
      <Category Name=""categoryA"" />
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.02S"">
        <StepName Format=""step1"" />
      </Step>
    </Scenario>
  </Feature>
  <Feature Name=""My feature2"">
    <Scenario Status=""Passed"" Name=""scenario1"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT0.02S"">
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