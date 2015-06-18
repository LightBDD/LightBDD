using System.Globalization;
using System.Linq;
using LightBDD.Execution;
using LightBDD.Results;
using LightBDD.SummaryGeneration;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    public class BDD_runner_concurrency_tests : SomeSteps
    {
        private readonly int _elementsCount = 1500;

        [Test]
        public void Running_scenarios_should_be_thread_safe()
        {
            var subject = new TestableBDDRunner(GetType(), new NoNotifier());

            Enumerable.Range(0, _elementsCount)
            .Select(i => ToScenarioBuilder(subject, i))
            .ToArray()
            .AsParallel()
            .ForAll(b => b.Run(Step_with_comment));

            Assert.That(subject.Result.Scenarios.Count(), Is.EqualTo(_elementsCount));
            for (int i = 0; i < _elementsCount; ++i)
            {
                var expectedName = ToName(i);
                var scenario = subject.Result.Scenarios.FirstOrDefault(s => s.Name == expectedName);
                Assert.That(scenario, Is.Not.Null, "Missing scenario: {0}", i);

                var step = scenario.Steps.FirstOrDefault();
                Assert.That(step, Is.Not.Null, "Missing step for scenario: {0}", i);
                Assert.That(step.Comments.ToArray(), Is.EqualTo(new[] { "comment one", "comment 2" }), "missing comments for scenario: {0}", i);
            }
        }

        [Test]
        public void SummaryGenerator_should_be_thread_safe()
        {
            var writer = MockRepository.GenerateMock<ISummaryWriter>();
            var subject = new SummaryGenerator(writer);

            Enumerable.Range(0, _elementsCount)
            .Select(i => MockRepository.GenerateMock<IFeatureResult>())
            .ToArray()
            .AsParallel()
            .ForAll(subject.AddFeature);

            subject.Finished();
            writer.AssertWasCalled(w => w.Save(Arg<IFeatureResult[]>.Matches(r => r.Length == _elementsCount)));
        }

        private static ICustomizedScenarioBuilder ToScenarioBuilder(TestableBDDRunner runner, int i)
        {
            return runner.NewScenario(ToName(i));
        }

        private static string ToName(int i)
        {
            return i.ToString(CultureInfo.InvariantCulture);
        }
    }
}