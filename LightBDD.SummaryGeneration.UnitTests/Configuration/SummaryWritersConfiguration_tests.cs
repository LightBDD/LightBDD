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
            Assert.That(configuration.Count, Is.EqualTo(1));

            Assert.That(configuration.Single(), Is.InstanceOf<SummaryFileWriter>());
            var writer = (SummaryFileWriter)configuration.Single();
            Assert.That(writer.OutputPath, Is.EqualTo(@"~\Reports\FeaturesSummary.xml"));
            Assert.That(writer.FullOutputPath, Is.EqualTo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + writer.OutputPath.Substring(1))));
            Assert.That(writer.Formatter, Is.InstanceOf<XmlResultFormatter>());
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
    }
}
