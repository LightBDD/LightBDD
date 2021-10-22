using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Framework.Messaging;
using LightBDD.Framework.UnitTests.Messaging.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Messaging
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class MessageListener_EnsureReceived_tests
    {
        private readonly MessageSource _source = new MessageSource();

        [Test]
        public async Task EnsureReceived_returns_latest_message_matching_criteria_if_it_has_been_already_received()
        {
            using var listener = MessageListener.Start(_source);
            _source.Publish(new TestMessage("001") { Text = "foo" });
            _source.Publish(new TestMessage("002") { Text = "bar" });
            var msg3 = _source.Publish(new TestMessage("003") { Text = "foo" });

            var actual = await listener.EnsureReceived<TestMessage>(x => x.Text == "foo");
            Assert.That(actual, Is.SameAs(msg3));
        }

        [Test]
        public async Task EnsureReceived_awaits_for_message_arrival_if_not_received_yet()
        {
            using var listener = MessageListener.Start(_source);
            var receiveTask = listener.EnsureReceived<TestMessage>(x => x.Id == "001");
            var msg = _source.Publish(new TestMessage("001"));

            var actual = await receiveTask;
            Assert.That(actual, Is.SameAs(msg));
        }

        [Test]
        public void EnsureReceived_throws_TimeoutException_if_message_did_not_arrive_on_time()
        {
            using var listener = MessageListener.Start(_source);
            var timeout = TimeSpan.FromSeconds(1);
            var ex = Assert.ThrowsAsync<TimeoutException>(
                () => listener.EnsureReceived<TestMessage>(x => x.Id == "001", timeout));
            Assert.That(ex.Message, Is.EqualTo($"Failed to receive {nameof(TestMessage)} with criteria x => (x.Id == \"001\") within {timeout} timeout"));
        }
    }
}