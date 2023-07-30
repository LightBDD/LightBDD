using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [Ignore("Either drop or implement as special single thread test execution mode")]
    //TODO: review
    public class RunnableScenario_synchronous_step_execution_tests
    {
        private readonly ThreadLocal<Guid> _threadLocal = new(Guid.NewGuid);

        [Test]
        public async Task It_should_execute_all_the_steps_in_same_thread_as_scenario()
        {
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;
            var stepThreadIds = new List<int>();

            var steps = Enumerable
                .Repeat(() => stepThreadIds.Add(Thread.CurrentThread.ManagedThreadId), 500)
                .ToArray();

            await TestableScenarioFactory.Default.RunScenario(r => r.Test().TestScenario(steps));

            Assert.That(stepThreadIds.Count, Is.EqualTo(steps.Length), "Not all steps were executed");
            Assert.That(stepThreadIds.Distinct().ToArray(), Is.EqualTo(new[] { currentThreadId }), "All steps should be executed within same thread as scenario");
        }

        [Test]
        public async Task It_should_allow_steps_to_use_ThreadLocalStorage_to_share_data()
        {
            var readSharedData = new List<Guid>();
            var steps = Enumerable
                .Repeat(() => readSharedData.Add(_threadLocal.Value), 500)
                .ToArray();

            await TestableScenarioFactory.Default.RunScenario(r => r.Test().TestScenario(steps));

            Assert.That(readSharedData.Count, Is.EqualTo(steps.Length), "Not all steps were executed");
            Assert.That(readSharedData.Distinct().Count(), Is.EqualTo(1), "All steps should be able to share data");
        }
    }
}