using System;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [NonParallelizable]
    public class FeatureCoordinatorTests
    {
        class TestableFeatureCoordinator : FrameworkFeatureCoordinator
        {
            public TestableFeatureCoordinator()
                : this(TestableIntegrationContextBuilder.Default()) { }

            public TestableFeatureCoordinator(TestableIntegrationContextBuilder builder)
                : base(builder.Build())
            {
            }

            public TestableFeatureCoordinator InstallSelf()
            {
                Install(this);
                return this;
            }

            public static FeatureCoordinator GetInstalled() => Instance;
        }

        class TestableConfigurationAwareReportWriter : LightBddConfigurationAware, IReportWriter
        {
            public void Save(params IFeatureResult[] results)
            {
                Assert.That(Configuration.ReportWritersConfiguration(), Does.Contain(this));
                WasExecuted = true;
            }

            public bool WasExecuted { get; private set; }
        }

        [Test]
        public void It_should_not_be_possible_to_install_multiple_coordinators_at_the_same_time()
        {
            using (var coord1 = new TestableFeatureCoordinator())
            using (var coord2 = new TestableFeatureCoordinator())
            {
                coord1.InstallSelf();
                var ex = Assert.Throws<InvalidOperationException>(() => coord2.InstallSelf());
                Assert.That(ex.Message, Is.EqualTo($"FeatureCoordinator of {typeof(TestableFeatureCoordinator)} type is already installed"));
            }
        }

        [Test]
        public void It_should_be_possible_to_install_new_cooridnator_when_previous_was_disposed()
        {
            using (new TestableFeatureCoordinator().InstallSelf())
            {
            }
            using (var coord = new TestableFeatureCoordinator())
                Assert.DoesNotThrow(() => coord.InstallSelf());
        }

        [Test]
        public void The_coordinator_should_uninstall_self_upon_disposal()
        {
            using (var coord = new TestableFeatureCoordinator().InstallSelf())
                Assert.That(TestableFeatureCoordinator.GetInstalled(), Is.SameAs(coord));
            Assert.That(TestableFeatureCoordinator.GetInstalled(), Is.Null);
        }

        [Test]
        public void The_coordinator_should_uninstall_self_upon_disposal_only_if_it_actively_used_one()
        {
            using (var installedCoordinator = new TestableFeatureCoordinator().InstallSelf())
            {
                using (new TestableFeatureCoordinator()) { }
                Assert.That(TestableFeatureCoordinator.GetInstalled(), Is.SameAs(installedCoordinator));
            }
        }

        [Test]
        public void Disposal_of_coordinator_should_dispose_DI_container()
        {
            var container = new Mock<IDependencyContainerV2>();
            var contextBuilder = TestableIntegrationContextBuilder.Default()
                .WithConfiguration(c => c.DependencyContainerConfiguration().UseContainer(container.Object));
            new TestableFeatureCoordinator(contextBuilder).Dispose();

            container.Verify(x => x.Dispose());
        }

        [Test]
        public void Disposal_of_coordinator_should_allow_execution_of_configuration_aware_report_formatters()
        {
            var writer = new TestableConfigurationAwareReportWriter();

            using (var _ = new TestableFeatureCoordinator(TestableIntegrationContextBuilder.Default().WithConfiguration(c => c.ReportWritersConfiguration().Add(writer)))
                .InstallSelf())
            {
            }
            Assert.That(writer.WasExecuted, Is.True);
        }
    }
}