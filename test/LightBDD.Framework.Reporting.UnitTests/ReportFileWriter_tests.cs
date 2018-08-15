using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Formatters;
using Moq;
using NUnit.Framework;
using System;
using System.IO;

namespace LightBDD.Framework.Reporting.UnitTests
{
    [TestFixture]
    public class ReportFileWriter_tests
    {
        [Test]
        public void It_should_use_appdomain_base_directory_if_output_starts_with_tilde()
        {
            var outputPath = "~" + Path.DirectorySeparatorChar + "output.txt";

            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(BaseDirectory + Path.DirectorySeparatorChar + "output.txt");
            Assert.That(writer.OutputPath, Is.EqualTo(outputPath));
            Assert.That(writer.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_use_working_directory_if_output_is_relative_path()
        {
            var outputPath = "output.txt";

            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(outputPath);
            Assert.That(writer.OutputPath, Is.EqualTo(outputPath));
            Assert.That(writer.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_preserve_output_path_if_it_is_absolute()
        {
            var outputPath = "c:" + Path.DirectorySeparatorChar + "output.txt";

            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(outputPath);
            Assert.That(writer.OutputPath, Is.EqualTo(outputPath));
            Assert.That(writer.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_preserve_output_path_if_it_is_unc()
        {
            var outputPath = @"\\machine\c$\reports\output.txt";

            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), outputPath);
            Assert.That(writer.OutputPath, Is.EqualTo(outputPath));
            Assert.That(writer.FullOutputPath, Is.EqualTo(outputPath));
        }

        [Test]
        public void It_should_preserve_output_path_but_with_working_directory_root_if_it_refers_to_root_directory()
        {
            var outputPath = Path.DirectorySeparatorChar + "output.txt";

            var writer = new ReportFileWriter(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(outputPath);
            Assert.That(writer.OutputPath, Is.EqualTo(outputPath));
            Assert.That(writer.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void Save_should_use_formatter_to_write_data()
        {
            var expectedFileContent = "text";
            var outputPath = "~" + Path.DirectorySeparatorChar + $"{Guid.NewGuid()}" + Path.DirectorySeparatorChar + "output.txt";
            var expectedPath = outputPath.Replace("~", BaseDirectory);

            var formatter = Mock.Of<IReportFormatter>();
            var results = new[]
            {
                Mock.Of<IFeatureResult>(),
                Mock.Of<IFeatureResult>()
            };

            Mock.Get(formatter)
                .Setup(f => f.Format(It.IsAny<Stream>(), results))
                .Callback((Stream stream, IFeatureResult[] r) =>
                {
                    using (var writer = new StreamWriter(stream))
                        writer.Write(expectedFileContent);
                });

            new ReportFileWriter(formatter, outputPath).Save(results);

            Assert.That(File.Exists(expectedPath), "File does not exists: {0}", expectedPath);
            Assert.That(File.ReadAllText(expectedPath), Is.EqualTo(expectedFileContent));
        }

        private static string BaseDirectory
        {
            get
            {
#if NET451
                return AppDomain.CurrentDomain.BaseDirectory;
#else 
                return AppContext.BaseDirectory;
#endif
            }
        }
    }
}
