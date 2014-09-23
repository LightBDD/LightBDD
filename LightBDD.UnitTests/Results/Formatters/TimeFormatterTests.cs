using System;
using LightBDD.Formatters;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
    [TestFixture]
    public class TimeFormatterTests
    {
        [Test]
        [TestCase(5, 00, 00, 00, 000, 000, 800, "5d")]
        [TestCase(5, 02, 03, 04, 006, 007, 800, "5d 02h")]
        [TestCase(0, 02, 03, 04, 006, 007, 800, "2h 03m")]
        [TestCase(0, 00, 03, 04, 006, 007, 800, "3m 04s")]
        [TestCase(0, 00, 00, 04, 006, 007, 800, "4s 006ms")]
        [TestCase(0, 00, 00, 00, 006, 007, 800, "6ms")]
        [TestCase(0, 00, 00, 00, 000, 007, 800, "<1ms")]
        [TestCase(0, 00, 00, 00, 000, 000, 800, "<1ms")]
        [TestCase(0, 00, 00, 00, 000, 000, 000, "0ms")]
        public void Should_format_pretty(int days, int hours, int minutes, int seconds, int milliseconds, int microseconds, int nanoseconds, string expected)
        {
            var microTime = TimeSpan.FromTicks(((microseconds * TimeSpan.TicksPerMillisecond) / 1000));
            var nanoTime = TimeSpan.FromTicks(((nanoseconds * TimeSpan.TicksPerMillisecond) / 1000000));

            var timeSpan = new TimeSpan(days, hours, minutes, seconds, milliseconds).Add(microTime).Add(nanoTime);
            Assert.That(timeSpan.FormatPretty(), Is.EqualTo(expected));
        }
    }
}
