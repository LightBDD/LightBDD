using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class ExecutionTimerTests
    {
        [Test]
        public async Task ExecutionTimer_should_start_measuring_upon_creation()
        {
            var start = DateTimeOffset.UtcNow;
            var watch = Stopwatch.StartNew();

            var timer = TestableIntegrationContextBuilder.Default().Build().ExecutionTimer;

            await Task.Delay(100);
            var time = timer.GetTime();

            var end = DateTimeOffset.UtcNow;
            var elapsed = watch.Elapsed;

            Assert.That(time.Start, Is.GreaterThanOrEqualTo(start).And.LessThanOrEqualTo(end));
            Assert.That(time.Offset, Is.LessThanOrEqualTo(elapsed));
        }

        [Test]
        public async Task ExecutionTimer_should_provide_time_series_while_keeping_the_start_date_the_same()
        {
            var timer = TestableIntegrationContextBuilder.Default().Build().ExecutionTimer;

            var time1 = timer.GetTime();
            await Task.Delay(50);
            var time2 = timer.GetTime();
            Assert.That(time2.Time, Is.GreaterThan(time1.Time));
            Assert.That(time2.Start, Is.EqualTo(time1.Start));


            await Task.Delay(50);
            var time3 = timer.GetTime();

            Assert.That(time3.Time, Is.GreaterThan(time2.Time));
            Assert.That(time3.Start, Is.EqualTo(time2.Start));
        }
    }
}
