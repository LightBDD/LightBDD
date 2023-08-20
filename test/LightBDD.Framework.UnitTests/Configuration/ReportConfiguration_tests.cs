using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Reporting;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Configuration
{
    [TestFixture]
    public class ReportConfiguration_tests
    {
        [Test]
        public void It_should_return_default_configuration()
        {
            var configuration = new LightBddConfiguration();
            configuration.Services.ConfigureReportGenerators()
                .AddFrameworkDefaultReportGenerators();

            var featuresReportHtml = $"~{Path.DirectorySeparatorChar}Reports{Path.DirectorySeparatorChar}FeaturesReport.html";

            AssertGenerator(configuration, featuresReportHtml, typeof(HtmlReportFormatter), featuresReportHtml.Replace("~", AppContext.BaseDirectory));
        }

        private void AssertGenerator(LightBddConfiguration configuration, string expectedRelativePath, Type expectedFormatterType, string expectedFullPath)
        {
            var writer = GetReportGenerators(configuration)
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
            var configuration = new LightBddConfiguration();
            configuration.Services.ConfigureReportGenerators().AddFrameworkDefaultReportGenerators();

            Assert.That(GetReportGeneratorDescriptors(configuration).Count(), Is.EqualTo(1));
            configuration.Services.ConfigureReportGenerators().Clear();

            Assert.That(GetReportGeneratorDescriptors(configuration), Is.Empty);
            configuration.Services.ConfigureReportGenerators().Add(gen1).Add(gen2);

            Assert.That(GetReportGeneratorDescriptors(configuration).Count(), Is.EqualTo(2));
        }

        [Test]
        public void It_should_allow_adding_file_report()
        {
            var cfg = new LightBddConfiguration();

            cfg.Services.ConfigureReportGenerators()
                .Clear()
                .AddFileReport<PlainTextReportFormatter>("file.txt");

            var generator = GetReportGenerators(cfg).SingleOrDefault();

            Assert.That(generator, Is.Not.Null);
            Assert.That(generator.Formatter, Is.TypeOf<PlainTextReportFormatter>());
            Assert.That(generator.OutputPath, Is.EqualTo("file.txt"));
        }

        [Test]
        public async Task It_should_return_default_file_attachment_manager()
        {
            var cfg = new LightBddConfiguration();
            cfg.Services.ConfigureDefaultFileAttachmentManager();

            await using var container = cfg.BuildContainer();
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

        private static IEnumerable<ServiceDescriptor> GetReportGeneratorDescriptors(LightBddConfiguration configuration)
        {
            return configuration.Services.Where(s => s.ServiceType == typeof(IReportGenerator));
        }

        private static IEnumerable<FileReportGenerator> GetReportGenerators(LightBddConfiguration configuration)
        {
            return GetReportGeneratorDescriptors(configuration)
                .Select(x => x.ImplementationFactory(Mock.Of<IServiceProvider>()))
                .OfType<FileReportGenerator>();
        }
    }
}
