using System;
using System.IO;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using Moq;
using NUnit.Framework;

namespace LightBDD.Reporting.UnitTests
{
    [TestFixture]
    public class ReportFileWriter_tests
    {
        [Test]
        public void It_should_use_appdomain_base_directory_if_output_starts_with_tilde()
        {
            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), "~\\output.txt");
            var expected = Path.GetFullPath(AppContext.BaseDirectory + "\\output.txt");
            Assert.That(writer.OutputPath, Is.EqualTo("~\\output.txt"));
            Assert.That(writer.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_use_working_directory_if_output_is_relative_path()
        {
            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), "output.txt");
            var expected = Path.GetFullPath("output.txt");
            Assert.That(writer.OutputPath, Is.EqualTo("output.txt"));
            Assert.That(writer.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_preserve_output_path_if_it_is_absolute()
        {
            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), "c:\\output.txt");
            var expected = Path.GetFullPath("c:\\output.txt");
            Assert.That(writer.OutputPath, Is.EqualTo("c:\\output.txt"));
            Assert.That(writer.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void Save_should_use_formatter_to_write_data()
        {
            var expectedFileContent = "text";
            var outputPath = $"~\\{Guid.NewGuid()}\\output.txt";
            var expectedPath = outputPath.Replace("~", AppContext.BaseDirectory);

            var formatter = Mock.Of<IReportFormatter>();
            var results = new[]
            {
                Mock.Of<IFeatureResult>(),
                Mock.Of<IFeatureResult>()
            };

            Mock.Get(formatter)
                .Setup(f => f.Format(It.IsAny<Stream>(), results))
                .Callback((Stream stream,IFeatureResult[] r) =>
                {
                    using (var writer = new StreamWriter(stream))
                        writer.Write(expectedFileContent);
                });

            new ReportFileWriter(formatter, outputPath).Save(results);

            Assert.That(File.Exists(expectedPath), "File does not exists: {0}", expectedPath);
            Assert.That(File.ReadAllText(expectedPath), Is.EqualTo(expectedFileContent));
        }
    }
}
