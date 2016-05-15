using System;
using System.IO;
using LightBDD.Core.Execution.Results;
using LightBDD.SummaryGeneration.Formatters;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.SummaryGeneration.UnitTests
{
    [TestFixture]
    public class SummaryFileWriterTests
    {
        [Test]
        public void It_should_use_appdomain_base_directory_if_output_starts_with_tilde()
        {
            var writer = new SummaryFileWriter(MockRepository.GenerateMock<IResultFormatter>(), "~//output.txt");
            var expected = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\output.txt");
            Assert.That(writer.OutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_use_working_directory_if_output_is_relative_path()
        {
            var writer = new SummaryFileWriter(MockRepository.GenerateMock<IResultFormatter>(), "output.txt");
            var expected = Path.GetFullPath("output.txt");
            Assert.That(writer.OutputPath, Is.EqualTo(expected));
        }

        [Test]
        public void Save_should_use_formatter_to_write_data()
        {
            var expectedFileContent = "text";
            var outputPath = $"~\\{Guid.NewGuid()}\\output.txt";
            var expectedPath = outputPath.Replace("~", AppDomain.CurrentDomain.BaseDirectory);

            var formatter = MockRepository.GenerateMock<IResultFormatter>();
            var results = new[]
            {
                MockRepository.GenerateMock<IFeatureResult>(),
                MockRepository.GenerateMock<IFeatureResult>()
            };

            formatter
                .Stub(f => f.Format(Arg<Stream>.Is.Anything, Arg<IFeatureResult[]>.Is.Same(results)))
                .WhenCalled(call =>
                {
                    using (var writer = new StreamWriter((Stream)call.Arguments[0]))
                        writer.Write(expectedFileContent);
                });

            new SummaryFileWriter(formatter, outputPath).Save(results);

            Assert.That(File.Exists(expectedPath), "File does not exists: {0}", expectedPath);
            Assert.That(File.ReadAllText(expectedPath), Is.EqualTo(expectedFileContent));
        }
    }
}
