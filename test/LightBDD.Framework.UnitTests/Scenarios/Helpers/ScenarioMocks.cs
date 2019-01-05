using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios;
using Moq;

namespace LightBDD.Framework.UnitTests.Scenarios.Helpers
{
    public static class ScenarioMocks
    {
        public static IBddRunner CreateBddRunner(params Mock<ICoreScenarioBuilder>[] builders)
        {
            var runner = new Mock<IBddRunner>();
            var sequence = runner.SetupSequence(x => x.Integrate());
            foreach (var builder in builders)
            {
                var integrated = new Mock<IIntegratedScenarioBuilder<NoContext>>();
                integrated.Setup(x => x.Core).Returns(builder.Object);
                integrated.Setup(x => x.RunAsync()).Returns(() => builder.Object.Build().Invoke());
                integrated.Setup(x => x.Integrate()).Returns(integrated.Object);
                sequence = sequence.Returns(integrated.Object);
            }

            return runner.Object;
        }

        public static Mock<ICoreScenarioBuilder> CreateScenarioBuilder() => new Mock<ICoreScenarioBuilder>();

        public static List<StepDescriptor> ExpectAddSteps(this Mock<ICoreScenarioBuilder> builder)
        {
            var capture = new List<StepDescriptor>();
            builder.Setup(x => x.AddSteps(It.IsAny<IEnumerable<StepDescriptor>>()))
                .Returns((IEnumerable<StepDescriptor> s) =>
                {
                    capture.AddRange(s);
                    return builder.Object;
                })
                .Verifiable();
            return capture;
        }

        public static Capture<Func<object>> ExpectContext(this Mock<ICoreScenarioBuilder> builder)
        {
            var capture = new Capture<Func<object>>();
            builder
                .Setup(s => s.WithContext(It.IsAny<Func<object>>(), It.IsAny<bool>()))
                .Returns((Func<object> obj, bool takeOwnership) =>
                {
                    capture.Value = obj;
                    return builder.Object;
                })
                .Verifiable();
            return capture;
        }

        public static Mock<ICoreScenarioBuilder> SetupConfiguration(this Mock<ICoreScenarioBuilder> builder, LightBddConfiguration cfg)
        {
            builder.Setup(x => x.Configuration).Returns(cfg);
            return builder;
        }

        public static Mock<ICoreScenarioBuilder> ExpectWithCapturedScenarioDetailsIfNotSpecified(this Mock<ICoreScenarioBuilder> builder)
        {
            builder
                .Setup(x => x.WithCapturedScenarioDetailsIfNotSpecified()).Returns(builder.Object)
                .Verifiable();
            return builder;
        }

        public static Mock<ICoreScenarioBuilder> ExpectWithCapturedScenarioDetails(this Mock<ICoreScenarioBuilder> builder)
        {
            builder
                .Setup(x => x.WithCapturedScenarioDetails()).Returns(builder.Object)
                .Verifiable();
            return builder;
        }

        public static Capture<bool> ExpectBuild(this Mock<ICoreScenarioBuilder> builder)
        {
            var capture = new Capture<bool>();
            builder
                .Setup(s => s.Build())
                .Returns(() =>
                {
                    capture.Value = true;
                    return Task.FromResult(0);
                })
                .Verifiable();
            return capture;
        }

        public static Capture<Func<IDependencyResolver, object>> ExpectResolvedContext(this Mock<ICoreScenarioBuilder> builder)
        {
            var capture = new Capture<Func<IDependencyResolver, object>>();
            builder
                .Setup(s => s.WithContext(It.IsAny<Func<IDependencyResolver, object>>(), null))
                .Returns((Func<IDependencyResolver, object> contextResolver, Action<ContainerConfigurator> _) =>
                {
                    capture.Value = contextResolver;
                    return builder.Object;
                })
                .Verifiable();
            return capture;
        }
    }
}