using System;
using System.Linq;
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

            var receiveTask = listener.EnsureReceivedMany<TestMessage>(4, x => x.Text == "foo");
            var msg4 = _source.Publish(new TestMessage("004") { Text = "foo" });
            var msg5 = _source.Publish(new TestMessage("005") { Text = "foo" });
            _source.Publish(new TestMessage("006") { Text = "foo" });

            var actual = await receiveTask;
            Assert.That(actual, Is.EqualTo(new[] { msg5, msg4, msg3, msg1 }));
        }

        [Test]
        public void EnsureReceivedMany_provides_message_details_on_predicate_failure_when_awaiting_new_message()
        {
            using var listener = MessageListener.Start(_source);
            var receiveTask = listener.EnsureReceivedMany<TestMessage>(2, m => m.Text.Length > 0);
            var msg = new TestMessage("000") { Text = null };
            _source.Publish(msg);
            var ex = Assert.ThrowsAsync<MessagePredicateEvaluationException>(() => receiveTask);
            Assert.That(ex.Message.Replace("\r", ""), Is.EqualTo($"Unable to evaluate predicate on message {nameof(TestMessage)}: Object reference not set to an instance of an object.\nFaulty message:\nTestMessage: {{ Id=\"000\" Text=null }}"));
            Assert.That(ex.MessageObject, Is.SameAs(msg));
        }

        [Test]
        public void EnsureReceivedMany_provides_message_details_on_predicate_failure_when_awaiting_already_received_message()
        {
            using var listener = MessageListener.Start(_source);
            var msg = new TestMessage("000") { Text = null };
            _source.Publish(msg);

            var ex = Assert.ThrowsAsync<MessagePredicateEvaluationException>(() => listener.EnsureReceivedMany<TestMessage>(2, m => m.Text.Length > 0));
            Assert.That(ex.Message.Replace("\r", ""), Is.EqualTo($"Unable to evaluate predicate on message {nameof(TestMessage)}: Object reference not set to an instance of an object.\nFaulty message:\nTestMessage: {{ Id=\"000\" Text=null }}"));
            Assert.That(ex.MessageObject, Is.SameAs(msg));
        }

        [Test]
        public void EnsureReceivedMany_throws_OperationCanceledException_when_cancellation_token_is_cancelled()
        {
            using var listener = MessageListener.Start(_source);

            Assert.ThrowsAsync<OperationCanceledException>(() =>
                listener.EnsureReceivedMany<TestMessage>(2, m => m.Id != null, cancellationToken: new CancellationTokenSource(100).Token));
        }

        [Test]
        public void EnsureReceivedMany_throws_OperationCanceledException_when_listener_is_disposed()
        {
            var listener = MessageListener.Start(_source);

            var receiveTask = listener.EnsureReceivedMany<TestMessage>(2, m => m.Id != null);
            listener.Dispose();
            Assert.ThrowsAsync<OperationCanceledException>(() => receiveTask);
        }

        [Test]
        public void EnsureReceivedMany_provides_custom_error_details_on_timeout()
        {
            using var listener = MessageListener.Start(_source);
            var timeout = TimeSpan.FromMilliseconds(100);
            _source.Publish(new TestMessage("01") { Text = "Abc" });
            _source.Publish(new TestMessage("02") { Text = "Abc" });
            _source.Publish(new TestMessage("03") { Text = "Abd" });
            var ex = Assert.ThrowsAsync<TimeoutException>(() => listener.EnsureReceivedMany<TestMessage>(3, x => x.Text == "Abc", timeout));
            Assert.That(ex.Message.Replace("\r", ""), Is.EqualTo(@"Failed to receive matching 3 TestMessage message(s) within 100ms:

Received 2 messages matching criteria:
TestMessage: { Id=""02"" Text=""Abc"" }
TestMessage: { Id=""01"" Text=""Abc"" }

Last recorded TestMessage messages:
TestMessage: { Id=""03"" Text=""Abd"" }
TestMessage: { Id=""02"" Text=""Abc"" }
TestMessage: { Id=""01"" Text=""Abc"" }".Replace("\r", "")));
        }
    }
}