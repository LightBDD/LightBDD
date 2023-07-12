using System;
using System.IO;
using System.Linq;
using LightBDD.Core.Configuration;
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
            Assert.That(configuration.Count, Is.EqualTo(1));

            var featuresReportHtml = $"~{Path.DirectorySeparatorChar}Reports{Path.DirectorySeparatorChar}FeaturesReport.html";

            AssertGenerator(configuration, featuresReportHtml, typeof(HtmlReportFormatter), featuresReportHtml.Replace("~", AppContext.BaseDirectory));
        }

        private void AssertGenerator(ReportConfiguration configuration, string expectedRelativePath, Type expectedFormatterType, string expectedFullPath)
        {
            var writer = configuration.OfType<FileReportGenerator>().FirstOrDefault(w => w.OutputPath == expectedRelativePath);
            Assert.That(writer, Is.Not.Null, $"Expected to find writer with path: {expectedRelativePath}");

            Assert.That(writer.FullOutputPath, Is.EqualTo(Path.GetFullPath(expectedFullPath)));
            Assert.That(writer.Formatter, Is.InstanceOf(expectedFormatterType));
        }

        [Test]
        public void It_should_allow_clear_add_and_remove_generators()
        {
            var gen1 = Mock.Of<IReportGenerator>();
            var gen2 = Mock.Of<IReportGenerator>();
            var configuration = new ReportConfiguration();
            Assert.That(configuration.Clear(), Is.Empty);
            Assert.That(configuration.Add(gen1).Add(gen2).ToArray(), Is.EqualTo(new[] { gen1, gen2 }));
            Assert.That(configuration.Remove(gen1).ToArray(), Is.EqualTo(new[] { gen2 }));
        }

        [Test]
        public void It_should_not_allow_null_generators()
        {
            var configuration = new ReportConfiguration();
            Assert.Throws<ArgumentNullException>(() => configuration.Add(null));
        }

        [Test]
        public void It_should_allow_adding_file_report_with_extension_method()
        {
            var writer = new ReportConfiguration()
                .Clear()
                .AddFileReport<PlainTextReportFormatter>("file.txt")
                .Cast<FileReportGenerator>()
                .SingleOrDefault();

            Assert.That(writer, Is.Not.Null);
            Assert.That(writer.Formatter, Is.TypeOf<PlainTextReportFormatter>());
            Assert.That(writer.OutputPath, Is.EqualTo("file.txt"));
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var writer = Mock.Of<IReportGenerator>();
            var lightBddConfig = new LightBddConfiguration();
            var cfg = lightBddConfig.Get<ReportConfiguration>().Add(writer);
            lightBddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.Add(Mock.Of<IReportGenerator>()));
            Assert.Throws<InvalidOperationException>(() => cfg.Clear());
            Assert.Throws<InvalidOperationException>(() => cfg.Remove(writer));
            Assert.Throws<InvalidOperationException>(() => cfg.UpdateFileAttachmentsManager(Mock.Of<IFileAttachmentsManager>()));
            Assert.That(cfg.ToArray(), Is.Not.Empty);
        }

        [Test]
        public void It_should_return_default_file_attachment_manager()
        {
            var configuration = new ReportConfiguration().RegisterDefaultFileAttachmentManager();
            var fileAttachmentsManager = configuration.GetFileAttachmentsManager();
            Assert.That(fileAttachmentsManager, Is.TypeOf<FileAttachmentsManager>());

            var expectedPath = Path.Combine(AppContext.BaseDirectory, "Reports");
            Assert.That(((FileAttachmentsManager)fileAttachmentsManager).AttachmentsDirectory, Is.EqualTo(expectedPath));
        }

        [Test]
        public void It_should_allow_updating_FileAttachmentManager()
        {
            var manager = Mock.Of<IFileAttachmentsManager>();
            var actual = new ReportConfiguration().UpdateFileAttachmentsManager(manager)
                .GetFileAttachmentsManager();
            Assert.That(actual, Is.EqualTo(manager));
        }

        [Test]
        public void It_should_not_allow_updating_FileAttachmentManager_with_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new ReportConfiguration().UpdateFileAttachmentsManager(null));
            Assert.That(ex.ParamName, Is.EqualTo("manager"));
        }

        [Test]
        public void GetFileAttachmentsManager_should_return_NoFileAttachmentsManager_by_default()
        {
            Assert.That(new ReportConfiguration().GetFileAttachmentsManager(), Is.TypeOf<NoFileAttachmentsManager>());
        }
    }
}
