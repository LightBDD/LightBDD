using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Reporting;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.Reporting.UnitTests.Configuration
{
    [TestFixture]
    public class ReportConfiguration_tests
    {
        [Test]
        public void It_should_return_default_configuration()
        {
            var configuration = new ReportConfiguration().RegisterFrameworkDefaultReportWriters();
            Assert.That(configuration.Generators.Count, Is.EqualTo(1));

            var featuresReportHtml = $"~{Path.DirectorySeparatorChar}Reports{Path.DirectorySeparatorChar}FeaturesReport.html";

            AssertGenerator(configuration, featuresReportHtml, typeof(HtmlReportFormatter), featuresReportHtml.Replace("~", AppContext.BaseDirectory));
        }

        private void AssertGenerator(ReportConfiguration configuration, string expectedRelativePath, Type expectedFormatterType, string expectedFullPath)
        {
            var writer = configuration.Generators.Select(x => x.ImplementationFactory(Mock.Of<IServiceProvider>()))
                .OfType<FileReportGenerator>()
                .FirstOrDefault(w => w.OutputPath == expectedRelativePath);
            Assert.That(writer, Is.Not.Null, $"Expected to find writer with path: {expectedRelativePath}");

            Assert.That(writer.FullOutputPath, Is.EqualTo(Path.GetFullPath(expectedFullPath)));
            Assert.That(writer.Formatter, Is.InstanceOf(expectedFormatterType));
        }

        [Test]
        public void It_should_allow_clear_and_add_generators()
        {
            var gen1 = Mock.Of<IReportGenerator>();
            var gen2 = Mock.Of<IReportGenerator>();
            var configuration = new ReportConfiguration().RegisterFrameworkDefaultReportWriters();
            Assert.That(configuration.Generators.Count, Is.EqualTo(1));
            Assert.That(configuration.Clear().Generators, Is.Empty);
            Assert.That(configuration.Add(c => c.Use(gen1)).Add(c => c.Use(gen2)).Generators.Count, Is.EqualTo(2));
        }

        [Test]
        public void It_should_allow_adding_file_report()
        {
            var writer = new ReportConfiguration()
                .Clear()
                .AddFileReport<PlainTextReportFormatter>("file.txt")
                .Generators.Select(x => x.ImplementationFactory(Mock.Of<IServiceProvider>()))
                .Cast<FileReportGenerator>()
                .SingleOrDefault();

            Assert.That(writer, Is.Not.Null);
            Assert.That(writer.Formatter, Is.TypeOf<PlainTextReportFormatter>());
            Assert.That(writer.OutputPath, Is.EqualTo("file.txt"));
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var generator = Mock.Of<IReportGenerator>();
            var lightBddConfig = new LightBddConfiguration();
            var cfg = lightBddConfig.Get<ReportConfiguration>().Add(c => c.Use(generator));
            lightBddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.Add(c => c.Use(Mock.Of<IReportGenerator>())));
            Assert.Throws<InvalidOperationException>(() => cfg.Clear());
            Assert.That(cfg.Generators.ToArray(), Is.Not.Empty);
        }

        [Test]
        public async Task It_should_return_default_file_attachment_manager()
        {
            await using var container = new LightBddConfiguration().RegisterDefaultFileAttachmentManager().BuildContainer();
            var fileAttachmentsManager = container.Resolve<IFileAttachmentsManager>();
            Assert.That(fileAttachmentsManager, Is.TypeOf<FileAttachmentsManager>());

            var expectedPath = Path.Combine(AppContext.BaseDirectory, "Reports");
            Assert.That(((FileAttachmentsManager)fileAttachmentsManager).AttachmentsDirectory, Is.EqualTo(expectedPath));
        }

        [Test]
        public async Task GetFileAttachmentsManager_should_return_NoFileAttachmentsManager_by_default()
        {
            await using var container = new LightBddConfiguration().BuildContainer();
            var fileAttachmentsManager = container.Resolve<IFileAttachmentsManager>();
            Assert.That(fileAttachmentsManager, Is.TypeOf<NoFileAttachmentsManager>());
        }
    }
}
