using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.SummaryGeneration;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.SummaryGeneration
{
    [TestFixture]
    public class FormattableSummaryFileWriterTests
    {
        private const string ExpectedContentText = "some expected text";
        private readonly DateTimeOffset _expectedExecutionStartOffset = DateTimeOffset.UtcNow;
        private IResultFormatter _formatter;
        private IFeatureResult _feature;
        private string _dirPath;
        private string _realDirPath;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _formatter = MockRepository.GenerateMock<IResultFormatter>();
            _feature = Mocks.CreateFeatureResult("name", "description", "label", Mocks.CreateScenarioResult("abc", "def", _expectedExecutionStartOffset, TimeSpan.Zero, new string[0], Mocks.CreateStepResult(1, ResultStatus.Passed)));
            _formatter.Stub(f => f.Format(Arg<IFeatureResult[]>.Matches(l => l.Contains(_feature)))).Return(ExpectedContentText);

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
            new FormattableSummaryFileWriter(_formatter, filePath).Save(_feature);
            var utcNow = DateTime.UtcNow;

            var expectedFileName = string.Format("{0:yyyy-MM-dd-HH_mm_ss}_{1:yyyy-MM-dd-HH_mm_ss}_{2:yyyy-MM-dd-HH_mm}_{3:yyyy-MM-dd-HH_mm}",
                    _expectedExecutionStartOffset.UtcDateTime, _expectedExecutionStartOffset.LocalDateTime,
                    utcNow, utcNow.ToLocalTime());


            var actualPath = GetActualPath();
            Assert.That(Path.GetFileName(actualPath), Is.EqualTo(expectedFileName));
            Assert.That(File.ReadAllText(actualPath), Is.EqualTo(ExpectedContentText));
        }

        [Test]
        public void Should_save_results_to_parameterized_file_name_with_tilde()
        {
            GenerateRelativeDirPathWithTilde();
            var filePath = PrepareFilePath("{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_{TestDateTime:yyyy-MM-dd-HH_mm_ss}_{CurrentDateTimeUtc:yyyy-MM-dd-HH_mm}_{CurrentDateTime:yyyy-MM-dd-HH_mm}");
            new FormattableSummaryFileWriter(_formatter, filePath).Save(_feature);
            var utcNow = DateTime.UtcNow;

            var expectedFileName = string.Format("{0:yyyy-MM-dd-HH_mm_ss}_{1:yyyy-MM-dd-HH_mm_ss}_{2:yyyy-MM-dd-HH_mm}_{3:yyyy-MM-dd-HH_mm}",
                    _expectedExecutionStartOffset.UtcDateTime, _expectedExecutionStartOffset.LocalDateTime,
                    utcNow, utcNow.ToLocalTime());


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
            _realDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + _dirPath;
        }

        private string PrepareFilePath(string name)
        {
            return string.Format("{0}{1}{2}", _dirPath, Path.DirectorySeparatorChar, name);
        }

    }
}