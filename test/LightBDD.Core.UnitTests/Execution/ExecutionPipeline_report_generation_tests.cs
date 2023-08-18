using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class ExecutionPipeline_report_generation_tests
{
    class MyFeature
    {
        [TestScenario]
        public void MyScenario() { }
    }

    [Test]
    public async Task It_should_use_all_registered_generators()
    {
        var generators = new List<Mock<IReportGenerator>>();
        void Configure(LightBddConfiguration cfg)
        {
            for (int i = 0; i < 3; ++i)
            {
                var gen = new Mock<IReportGenerator>();
                gen.Setup(x => x.Generate(It.IsAny<ITestRunResult>())).Returns(Task.CompletedTask).Verifiable();

                cfg.RegisterReportGenerators().Add(gen.Object);
                generators.Add(gen);
            }
        }

        var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(typeof(MyFeature));
        result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

        foreach (var mock in generators)
            mock.Verify();
    }

    [Test]
    public void It_should_generate_all_reports_and_throw_AggregateException_for_failed_ones()
    {
        void Configure(LightBddConfiguration cfg)
        {
            for (int i = 0; i < 3; ++i)
            {
                var gen = new Mock<IReportGenerator>();
                gen.Setup(x => x.Generate(It.IsAny<ITestRunResult>())).ThrowsAsync(new Exception($"{i}"));
                cfg.RegisterReportGenerators().Add(gen.Object);
            }
        }

        var ex = Assert.ThrowsAsync<AggregateException>(() => TestableCoreExecutionPipeline.Create(Configure).Execute(typeof(MyFeature)));
        ex.InnerExceptions.Select(e => e.Message).ToArray().ShouldBeEquivalentTo(new[] { "0", "1", "2" });
    }

    [Test]
    public async Task It_should_allow_report_generators_with_DI_dependencies()
    {
        var capture = new CollectionCapture<Dependency>();
        void Configure(LightBddConfiguration cfg)
        {
            cfg.Services.AddSingleton(capture);
            cfg.Services.AddTransient<Dependency>();
            cfg.RegisterReportGenerators().Add<ReportGeneratorWithDependency>(ServiceLifetime.Singleton);
            cfg.RegisterReportGenerators().Add<ReportGeneratorWithDependency>(ServiceLifetime.Scoped);
            cfg.RegisterReportGenerators().Add<ReportGeneratorWithDependency>(ServiceLifetime.Transient);
        }

        var result = await TestableCoreExecutionPipeline.Create(Configure)
            .Execute(typeof(MyFeature));
        result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

        capture.Count.ShouldBe(3);
        capture.ShouldAllBe(x => x.Disposed);
    }

    class Dependency : IDisposable
    {
        public bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
    }
    class CollectionCapture<T> : List<T> { }
    class ReportGeneratorWithDependency : IReportGenerator
    {
        private readonly Dependency _dep;

        public ReportGeneratorWithDependency(Dependency dep, CollectionCapture<Dependency> capture)
        {
            _dep = dep;
            capture.Add(dep);
        }

        public Task Generate(ITestRunResult result)
        {
            _dep.Disposed.ShouldBeFalse();
            return Task.CompletedTask;
        }
    }
}