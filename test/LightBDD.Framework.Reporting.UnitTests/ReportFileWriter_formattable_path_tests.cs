using System;
using System.IO;
using System.Linq;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.Reporting.UnitTests
{
    [TestFixture]
    public class ReportFileWriter_formattable_path_tests
    {
        private const string ExpectedContentText = "some expected text";
        private readonly DateTimeOffset _expectedExecutionStartOffset = DateTimeOffset.UtcNow;
        private Mock<IReportFormatter> _formatter;
        private ITestRunResult _testResult;
        private string _dirPath;
        private string _realDirPath;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _formatter = new Mock<IReportFormatter>();
            _testResult = TestResults.CreateTestRunResults(TestResults.CreateFeatureResult("name", "description", "label",
                TestResults.CreateScenarioResult("abc", "def", _expectedExecutionStartOffset, TimeSpan.Zero, Array.Empty<string>(),
                    TestResults.CreateStepResult(ExecutionStatus.Passed).WithStepNameDetails(1, "foo", "foo"))));

            _formatter.Setup(f => f.Format(It.IsAny<Stream>(), _testResult)).Callback(
                (Stream s, ITestRunResult _) =>
                {
                    using var writer = new StreamWriter(s);
                    writer.Write(ExpectedContentText);
                });

            _realDirPath = _dirPath = null;
        }

        [TearDown]
        public void TearDown()
        {
            if (_realDirPath != null)
                Directory.Delete(_realDirPath, true);
        }

        #endregion

        [Test]
        public void Should_save_results_to_parameterized_file_name()
        {
            GenerateRelativeDirPath();
            var filePath = PrepareFilePath("{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_{TestDateTime:yyyy-MM-dd-HH_mm_ss}_{CurrentDateTimeUtc:yyyy-MM-dd-HH_mm}_{CurrentDateTime:yyyy-MM-dd-HH_mm}");
            new FileReportGenerator(_formatter.Object, filePath).Generate(_testResult);
            var utcNow = DateTime.UtcNow;

            var expectedFileName = $"{_expectedExecutionStartOffset.UtcDateTime:yyyy-MM-dd-HH_mm_ss}_{_expectedExecutionStartOffset.LocalDateTime:yyyy-MM-dd-HH_mm_ss}_{utcNow:yyyy-MM-dd-HH_mm}_{utcNow.ToLocalTime():yyyy-MM-dd-HH_mm}";


            var actualPath = GetActualPath();
            Assert.That(Path.GetFileName(actualPath), Is.EqualTo(expectedFileName));
            Assert.That(File.ReadAllText(actualPath), Is.EqualTo(ExpectedContentText));
        }

        [Test]
        public void Should_save_results_to_parameterized_file_name_with_tilde()
        {
            GenerateRelativeDirPathWithTilde();
            var filePath = PrepareFilePath("{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_{TestDateTime:yyyy-MM-dd-HH_mm_ss}_{CurrentDateTimeUtc:yyyy-MM-dd-HH_mm}_{CurrentDateTime:yyyy-MM-dd-HH_mm}");
            new FileReportGenerator(_formatter.Object, filePath).Generate(_testResult);
            var utcNow = DateTime.UtcNow;

            var expectedFileName = $"{_expectedExecutionStartOffset.UtcDateTime:yyyy-MM-dd-HH_mm_ss}_{_expectedExecutionStartOffset.LocalDateTime:yyyy-MM-dd-HH_mm_ss}_{utcNow:yyyy-MM-dd-HH_mm}_{utcNow.ToLocalTime():yyyy-MM-dd-HH_mm}";


            var actualPath = GetActualPath();
            Assert.That(Path.GetFileName(actualPath), Is.EqualTo(expectedFileName));
            Assert.That(File.ReadAllText(actualPath), Is.EqualTo(ExpectedContentText));
        }

        private string GetActualPath()
        {
            return Directory.EnumerateFileSystemEntries(_realDirPath, "*", SearchOption.AllDirectories).Single();
        }

        private void GenerateRelativeDirPath()
        {
            _realDirPath = _dirPath = Guid.NewGuid().ToString();
        }

        private void GenerateRelativeDirPathWithTilde()
        {
            _dirPath = Guid.NewGuid().ToString();
            _realDirPath = AppContext.BaseDirectory + Path.DirectorySeparatorChar + _dirPath;
            _dirPath = "~" + Path.DirectorySeparatorChar + _dirPath;
        }

        private string PrepareFilePath(string name)
        {
            return $"{_dirPath}{Path.DirectorySeparatorChar}{name}";
        }

    }
}