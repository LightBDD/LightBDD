using System;
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
    public class SummaryFileWriterTests
    {
        private IResultFormatter _formatter;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _formatter = MockRepository.GenerateMock<IResultFormatter>();
        }

        #endregion

        [Test]
        public void Should_save_results_to_file_even_if_directory_does_not_exist()
        {
            var dirPath = Guid.NewGuid().ToString();
            var filePath = string.Format("{0}{1}{2}", dirPath, Path.DirectorySeparatorChar, Guid.NewGuid());
            var subject = new SummaryFileWriter(_formatter, filePath);


            const string expectedText = "some expected text";
            var feature = Mocks.CreateFeatureResult("name", "description", "label");
            try
            {
                _formatter.Stub(f => f.Format(Arg<IFeatureResult[]>.Matches(l => l.Contains(feature)))).Return(expectedText);
                subject.Save(feature);
                Assert.That(File.ReadAllText(filePath), Is.EqualTo(expectedText));
            }
            finally
            {
                File.Delete(filePath);
                Directory.Delete(dirPath);
            }
        }

        [Test]
        public void It_should_resolve_paths_starting_with_tilde_to_current_AppDomain_BaseDirectory()
        {
            var dirName = Guid.NewGuid().ToString();
            var filePath = string.Format("{0}{1}{2}", dirName, Path.DirectorySeparatorChar, Guid.NewGuid());
            var pathWithTilde = "~\\" + filePath;
            var expectedPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + filePath;
            var subject = new SummaryFileWriter(_formatter, pathWithTilde);


            const string expectedText = "some expected text";
            var feature = Mocks.CreateFeatureResult("name", "description", "label");
            try
            {
                _formatter.Stub(f => f.Format(Arg<IFeatureResult[]>.Matches(l => l.Contains(feature)))).Return(expectedText);
                subject.Save(feature);
                Assert.That(File.ReadAllText(expectedPath), Is.EqualTo(expectedText));
            }
            finally
            {
                File.Delete(expectedPath);
                Directory.Delete(dirName);
            }
        }
    }
}