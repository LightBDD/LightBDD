using System;
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
            .ForAll(b => b.Run(new Action[0]));

            Assert.That(subject.Result.Scenarios.Count(), Is.EqualTo(_elementsCount));
            for (int i = 0; i < _elementsCount; ++i)
            {
                var expectedName = ToName(i);
                Assert.That(subject.Result.Scenarios.Any(s => s.Name == expectedName), Is.True, "Missing scenario: {0}", i);
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