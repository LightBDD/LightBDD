using System;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Framework.Messaging;
using LightBDD.Framework.UnitTests.Messaging.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Messaging
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class MessageListener_EnsureReceivedMany_tests
    {
        private readonly MessageSource _source = new MessageSource();

        [Test]
        public async Task EnsureReceivedMany_returns_specified_amount_of_messages_matching_criteria_in_order_from_latest_to_oldest()
        {
            using var listener = MessageListener.Start(_source);
            var msg1 = _source.Publish(new TestMessage("001") { Text = "foo" });
            _source.Publish(new TestMessage("002") { Text = "bar" });
            var msg3 = _source.Publish(new TestMessage("003") { Text = "foo" });

            var receiveTask = listener.EnsureReceivedMany<TestMessage>(4, x => x.Text == "foo", "boom");
            var msg4 = _source.Publish(new TestMessage("004") { Text = "foo" });
            var msg5 = _source.Publish(new TestMessage("005") { Text = "foo" });
            _source.Publish(new TestMessage("006") { Text = "foo" });

            var actual = await receiveTask;
            Assert.That(actual, Is.EqualTo(new[] { msg5, msg4, msg3, msg1 }));
        }

    }
}