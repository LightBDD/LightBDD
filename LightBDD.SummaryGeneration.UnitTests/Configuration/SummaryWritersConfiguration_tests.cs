using System;
using System.IO;
using System.Linq;
using LightBDD.SummaryGeneration.Configuration;
using LightBDD.SummaryGeneration.Formatters;
using Moq;
using NUnit.Framework;

namespace LightBDD.SummaryGeneration.UnitTests.Configuration
{
    [TestFixture]
    public class SummaryWritersConfiguration_tests
    {
        [Test]
        public void It_should_return_default_configuration()
        {
            var configuration = new SummaryWritersConfiguration();
            Assert.That(configuration.Count, Is.EqualTo(2));

            var featuresSummaryXml = @"~\Reports\FeaturesSummary.xml";
            var featuresSummaryHtml = @"~\Reports\FeaturesSummary.html";

            AssertWriter(configuration, featuresSummaryXml, typeof(XmlResultFormatter), featuresSummaryXml.Replace("~", AppDomain.CurrentDomain.BaseDirectory));
            AssertWriter(configuration, featuresSummaryHtml, typeof(HtmlResultFormatter), featuresSummaryHtml.Replace("~", AppDomain.CurrentDomain.BaseDirectory));
        }

        private void AssertWriter(SummaryWritersConfiguration configuration, string expectedRelativePath, Type expectedFormatterType, string expectedFullPath)
        {
            var writer = configuration.OfType<SummaryFileWriter>().FirstOrDefault(w => w.OutputPath == expectedRelativePath);
            Assert.That(writer, Is.Not.Null, $"Expected to find writer with path: {expectedRelativePath}");

            Assert.That(writer.FullOutputPath, Is.EqualTo(Path.GetFullPath(expectedFullPath)));
            Assert.That(writer.Formatter, Is.InstanceOf(expectedFormatterType));
        }

        [Test]
        public void It_should_allow_clear_add_and_remove_items()
        {
            var writer = Mock.Of<ISummaryWriter>();
            var writer2 = Mock.Of<ISummaryWriter>();
            var configuration = new SummaryWritersConfiguration();
            Assert.That(configuration.Clear(), Is.Empty);
            Assert.That(configuration.Add(writer).Add(writer2).ToArray(), Is.EqualTo(new[] { writer, writer2 }));
            Assert.That(configuration.Remove(writer).ToArray(), Is.EqualTo(new[] { writer2 }));
        }

        [Test]
        public void It_should_not_allow_null_items()
        {
            var configuration = new SummaryWritersConfiguration();
            Assert.Throws<ArgumentNullException>(() => configuration.Add(null));
        }

        [Test]
        public void It_should_allow_adding_file_writer_with_extension_method()
        {
            var writer = new SummaryWritersConfiguration()
                .Clear()
                .AddFileWriter<PlainTextResultFormatter>("file.txt")
                .Cast<SummaryFileWriter>()
                .SingleOrDefault();

            Assert.That(writer, Is.Not.Null);
            Assert.That(writer.Formatter, Is.TypeOf<PlainTextResultFormatter>());
            Assert.That(writer.OutputPath, Is.EqualTo("file.txt"));
        }
    }
}
