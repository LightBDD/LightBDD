using System;
using System.IO;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Reporting
{
    [TestFixture]
    public class FileReportGenerator_tests
    {
        [Test]
        public void It_should_use_appdomain_base_directory_if_output_starts_with_tilde()
        {
            var outputPath = "~" + Path.DirectorySeparatorChar + "output.txt";

            var generator = new FileReportGenerator(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(AppContext.BaseDirectory + Path.DirectorySeparatorChar + "output.txt");
            Assert.That(generator.OutputPath, Is.EqualTo(outputPath));
            Assert.That(generator.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_use_working_directory_if_output_is_relative_path()
        {
            var outputPath = "output.txt";

            var generator = new FileReportGenerator(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(outputPath);
            Assert.That(generator.OutputPath, Is.EqualTo(outputPath));
            Assert.That(generator.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_preserve_output_path_if_it_is_absolute()
        {
            var outputPath = "c:" + Path.DirectorySeparatorChar + "output.txt";

            var generator = new FileReportGenerator(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(outputPath);
            Assert.That(generator.OutputPath, Is.EqualTo(outputPath));
            Assert.That(generator.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_preserve_output_path_if_it_is_unc()
        {
            var outputPath = @"\\machine\c$\reports\output.txt";

            var generator = new FileReportGenerator(Mock.Of<IReportFormatter>(), outputPath);
            Assert.That(generator.OutputPath, Is.EqualTo(outputPath));
            Assert.That(generator.FullOutputPath, Is.EqualTo(outputPath));
        }

        [Test]
        public void It_should_preserve_output_path_but_with_working_directory_root_if_it_refers_to_root_directory()
        {
            var outputPath = Path.DirectorySeparatorChar + "output.txt";

            var generator = new FileReportGenerator(Mock.Of<IReportFormatter>(), outputPath);
            var expected = Path.GetFullPath(outputPath);
            Assert.That(generator.OutputPath, Is.EqualTo(outputPath));
            Assert.That(generator.FullOutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void Generate_should_use_formatter_to_write_data()
        {
            var expectedFileContent = "text";
            var outputPath = "~" + Path.DirectorySeparatorChar + $"{Guid.NewGuid()}" + Path.DirectorySeparatorChar + "output.txt";
            var expectedPath = outputPath.Replace("~", AppContext.BaseDirectory);

            var formatter = Mock.Of<IReportFormatter>();
            var result = Mock.Of<ITestRunResult>();

            Mock.Get(formatter)
                .Setup(f => f.Format(It.IsAny<Stream>(), result))
                .Callback((Stream stream, ITestRunResult _) =>
                {
                    using var writer = new StreamWriter(stream);
                    writer.Write(expectedFileContent);
                });

            new FileReportGenerator(formatter, outputPath).Generate(result);

            Assert.That(File.Exists(expectedPath), "File does not exists: {0}", expectedPath);
            Assert.That(File.ReadAllText(expectedPath), Is.EqualTo(expectedFileContent));
        }
    }
}
