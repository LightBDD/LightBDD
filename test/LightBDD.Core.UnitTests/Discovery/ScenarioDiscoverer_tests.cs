using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Discovery;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Discovery
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class ScenarioDiscoverer_tests
    {
        [Test]
        public void It_should_discover_scenario_methods()
        {
            var type = typeof(DerivedTestFixture).GetTypeInfo();
            var scenarios = new ScenarioDiscoverer().DiscoverFor(type).ToArray();

            scenarios.AssertEquivalentTo(
                ScenarioCase.CreateParameterless(type, GetMethod(type, nameof(DerivedTestFixture.Scenario1))),
                ScenarioCase.CreateParameterless(type, GetMethod(type, nameof(DerivedTestFixture.Scenario2))),
                ScenarioCase.CreateParameterless(type, GetMethod(type, nameof(DerivedTestFixture.Scenario3))));
        }

        [Test]
        [TestCase(typeof(DerivedTestFixture))]
        [TestCase(typeof(ParameterizedTestFixture))]
        public void It_should_cancel_discovery_with_cancellation_token(Type type)
        {
            var cases = new List<ScenarioCase>();
            var ctx = new CancellationTokenSource();
            foreach (var scenarioCase in new ScenarioDiscoverer().DiscoverFor(type.GetTypeInfo(), ctx.Token))
            {
                cases.Add(scenarioCase);
                ctx.Cancel();
            }
            cases.Count.ShouldBe(1);
        }

        [Test]
        public void It_should_discover_parameterized_scenarios()
        {
            var type = typeof(ParameterizedTestFixture).GetTypeInfo();
            var scenarios = new ScenarioDiscoverer().DiscoverFor(type).ToArray();

            scenarios.AssertEquivalentTo(
                ScenarioCase.CreateParameterized(type, GetMethod(type, nameof(ParameterizedTestFixture.Scenario1)), new object[] { 1, "text", 'A' }),
                ScenarioCase.CreateParameterized(type, GetMethod(type, nameof(ParameterizedTestFixture.Scenario1)), new object[] { 2, "boom", 'C' }),
                ScenarioCase.CreateParameterized(type, GetMethod(type, nameof(ParameterizedTestFixture.Scenario2)), new object[] { 10, "t", 'B' }),
                ScenarioCase.CreateParameterizedAtRuntime(type, GetMethod(type, nameof(ParameterizedTestFixture.Scenario2))));
        }

        [Test]
        public void It_should_ignore_abstract_and_open_generic_types_but_support_classes_deriving_from_them()
        {
            var disco = new ScenarioDiscoverer();
            disco.DiscoverFor(typeof(GenericFixture<>).GetTypeInfo()).ToArray().ShouldBeEmpty();
            disco.DiscoverFor(typeof(AbstractFixture).GetTypeInfo()).ToArray().ShouldBeEmpty();

            var closedGenericType = typeof(ClosedGenericFixture).GetTypeInfo();
            disco.DiscoverFor(closedGenericType).AssertEquivalentTo(
                ScenarioCase.CreateParameterized(closedGenericType, GetMethod(closedGenericType, nameof(ClosedGenericFixture.ScenarioB)), new object[] { null }));

            var concreteType = typeof(ConcreteFixture).GetTypeInfo();
            disco.DiscoverFor(concreteType).AssertEquivalentTo(
                ScenarioCase.CreateParameterless(concreteType, GetMethod(concreteType, nameof(ConcreteFixture.ScenarioA))));
        }

        private MethodInfo GetMethod(TypeInfo type, string name) => type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        class ParameterizedTestFixture
        {
            [TestScenario]
            [TestScenarioInlineCase(1, "text", 'A')]
            [TestScenarioInlineCase(2, "boom", 'C')]
            public void Scenario1(int a, string b, char c) { }

            [TestScenario]
            [TestScenarioInlineCase(10, "t", 'B')]
            [DynamicTestCase]
            public void Scenario2(int a, string b, char c) { }
        }

        class TestFixture
        {
            [TestScenario]
            public void Scenario1() { }
            [TestScenario]
            public Task Scenario2() => Task.CompletedTask;
            [TestScenario]
            protected void InvalidScenario1() { }
            [TestScenario]
            private void InvalidScenario2() { }
            [TestScenario]
            internal void InvalidScenario3() { }
            private void NonScenario1() { }
            public void NonScenario2() { }
            [TestScenario]
            public void InvalidScenario4<T>() { }
        }

        abstract class AbstractFixture
        {
            [TestScenario]
            public void ScenarioA() { }
        }

        class GenericFixture<T>
        {
            [TestScenario]
            [TestScenarioInlineCase(null)]
            public void ScenarioB(T param) { }
        }

        class ClosedGenericFixture : GenericFixture<string> { }
        class ConcreteFixture : AbstractFixture { }

        class DerivedTestFixture : TestFixture
        {
            [TestScenario]
            public void Scenario3() { }
        }

        class DynamicTestCaseAttribute : Attribute, IScenarioCaseSourceAttribute
        {
            public IEnumerable<object[]> GetCases()
            {
                yield return new object[] { -1, "x", 'X' };
            }

            public bool IsResolvableAtDiscovery => false;
        }
    }
}
