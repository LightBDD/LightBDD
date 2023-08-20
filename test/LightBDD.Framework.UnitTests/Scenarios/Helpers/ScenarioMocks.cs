using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IBddRunner CreateBddRunner(params Mock<ICoreScenarioStepsRunner>[] builders)
            => CreateBddRunner(builders.Select(b => b.Object));
        public static IBddRunner CreateBddRunner(IEnumerable<ICoreScenarioStepsRunner> builders)
        {
            var runner = new Mock<IBddRunner>();
            var sequence = runner.SetupSequence(x => x.Integrate());
            foreach (var builder in builders)
            {
                var integrated = new Mock<IIntegratedScenarioBuilder<NoContext>>();
                integrated.Setup(x => x.Core).Returns(builder);
                integrated.Setup(x => x.RunAsync()).Returns(() => builder.RunAsync());
                integrated.Setup(x => x.Integrate()).Returns(integrated.Object);
                sequence = sequence.Returns(integrated.Object);
            }

            return runner.Object;
        }

        public static Mock<ICoreScenarioStepsRunner> CreateScenarioBuilder() => new();

        public static List<StepDescriptor> ExpectAddSteps(this Mock<ICoreScenarioStepsRunner> builder)
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

        public static Capture<ExecutionContextDescriptor> ExpectContext(this Mock<ICoreScenarioStepsRunner> builder)
        {
            var capture = new Capture<ExecutionContextDescriptor>();
            builder
                .Setup(s => s.WithContext(It.IsAny<ExecutionContextDescriptor>()))
                .Returns((ExecutionContextDescriptor d) =>
                {
                    capture.Value = d;
                    return builder.Object;
                })
                .Verifiable();
            return capture;
        }

        public static Mock<ICoreScenarioStepsRunner> SetupConfiguration(this Mock<ICoreScenarioStepsRunner> builder, LightBddConfiguration cfg)
        {
            builder.Setup(x => x.Configuration).Returns(cfg);
            return builder;
        }

        public static Capture<bool> ExpectRun(this Mock<ICoreScenarioStepsRunner> builder)
        {
            var capture = new Capture<bool>();

            builder
                .Setup(s => s.RunAsync())
                .Returns(() =>
                {
                    capture.Value = true;
                    return Task.CompletedTask;
                })
                .Verifiable();
            return capture;
        }

        public static Capture<Func<IDependencyResolver, object>> ExpectResolvedContext(this Mock<ICoreScenarioStepsRunner> builder)
        {
            var capture = new Capture<Func<IDependencyResolver, object>>();
            builder
                .Setup(s => s.WithContext(It.IsAny<ExecutionContextDescriptor>()))
                .Returns((ExecutionContextDescriptor d) =>
                {
                    capture.Value = d.ContextResolver;
                    return builder.Object;
                })
                .Verifiable();
            return capture;
        }
    }
}