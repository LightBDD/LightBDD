using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Reporting
{
    [TestFixture]
    public class FeatureReportGenerator_tests
    {
        [Test]
        public async Task It_should_use_all_registered_generators()
        {
            var result = Mock.Of<ITestRunResult>();
            var cfg = new LightBddConfiguration();
            var generators = new List<Mock<IReportGenerator>>();

            for (int i = 0; i < 3; ++i)
            {
                var gen = new Mock<IReportGenerator>();
                gen.Setup(x => x.Generate(result)).Returns(Task.CompletedTask).Verifiable();

                cfg.ReportConfiguration().Add(c => c.Use(gen.Object));
                generators.Add(gen);
            }

            await using var container = cfg.BuildContainer();
            var generator = new FeatureReportGenerator(container.Resolve<IEnumerable<IReportGenerator>>());
            await generator.GenerateReports(result);

            foreach (var mock in generators)
                mock.Verify();
        }

        [Test]
        public async Task It_should_generate_all_reports_and_throw_AggregateException_for_failed_ones()
        {
            var result = Mock.Of<ITestRunResult>();
            var cfg = new LightBddConfiguration();

            for (int i = 0; i < 3; ++i)
            {
                var gen = new Mock<IReportGenerator>();
                gen.Setup(x => x.Generate(result)).ThrowsAsync(new Exception($"{i}"));
                cfg.ReportConfiguration().Add(c => c.Use(gen.Object));
            }

            await using var container = cfg.BuildContainer();
            var generator = new FeatureReportGenerator(container.Resolve<IEnumerable<IReportGenerator>>());
            var ex = Assert.ThrowsAsync<AggregateException>(() => generator.GenerateReports(result));
            ex.InnerExceptions.Select(e => e.Message).ToArray().ShouldBeEquivalentTo(new[] { "0", "1", "2" });
        }
    }
}