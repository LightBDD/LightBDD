using System;
using LightBDD.Core.Execution;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class EventTimeTests
    {
        [Test]
        public void Time_should_be_calculated_from_start_and_offset()
        {
            var start = DateTimeOffset.UtcNow;
            var offset = TimeSpan.FromMilliseconds(32674);
            var eventTime = new EventTime(start, offset);
            Assert.That(eventTime.Time, Is.EqualTo(start + offset));
        }

        [Test]
        public void GetExecutionTime_should_calculate_time_between_two_events_no_matter_of_order()
        {
            var utcNow = DateTimeOffset.UtcNow;
            var e1 = new EventTime(utcNow, TimeSpan.FromMilliseconds(300));
            var e2 = new EventTime(utcNow.AddMilliseconds(100), TimeSpan.FromMilliseconds(600));

            var executionTime = e1.GetExecutionTime(e2);
            Assert.That(executionTime.Start, Is.EqualTo(e1.Time));
            Assert.That(executionTime.Duration, Is.EqualTo(e2.Time - e1.Time));

            var executionTime2 = e2.GetExecutionTime(e1);
            Assert.That(executionTime2.Start, Is.EqualTo(executionTime.Start));
            Assert.That(executionTime2.Duration, Is.EqualTo(executionTime.Duration));
        }
    }
}
