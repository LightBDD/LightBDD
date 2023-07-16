using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_synchronous_step_execution_tests
    {
        private readonly ThreadLocal<Guid> _threadLocal = new(Guid.NewGuid);
        private ICoreScenarioBuilder CreateScenarioBuilder() => TestableExecutionPipeline.Default.CreateScenarioBuilder(this);

        [Test]
        public void It_should_execute_all_the_steps_in_same_thread_as_scenario()
        {
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;
            var stepThreadIds = new List<int>();

            var steps = Enumerable
                .Repeat(() => stepThreadIds.Add(Thread.CurrentThread.ManagedThreadId), 500)
                .ToArray();

            CreateScenarioBuilder().Test().TestScenarioPurelySync(steps);

            Assert.That(stepThreadIds.Count, Is.EqualTo(steps.Length), "Not all steps were executed");
            Assert.That(stepThreadIds.Distinct().ToArray(), Is.EqualTo(new[] { currentThreadId }), "All steps should be executed within same thread as scenario");
        }

        [Test]
        public void It_should_allow_steps_to_use_ThreadLocalStorage_to_share_data()
        {
            var readSharedData = new List<Guid>();
            var steps = Enumerable
                .Repeat(() => readSharedData.Add(_threadLocal.Value), 500)
                .ToArray();

            CreateScenarioBuilder().Test().TestScenarioPurelySync(steps);

            Assert.That(readSharedData.Count, Is.EqualTo(steps.Length), "Not all steps were executed");
            Assert.That(readSharedData.Distinct().Count(), Is.EqualTo(1), "All steps should be able to share data");
        }

        [Test]
        public void Integration_should_not_allow_running_asynchronous_tests_synchronously()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateScenarioBuilder().Test().TestScenarioPurelySync(TestStep.CreateAsync(() => { })));
            Assert.That(ex.Message, Is.EqualTo("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps."));
        }
    }
}