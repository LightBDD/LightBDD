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
            var timeout = TimeSpan.FromMilliseconds(100);
            Assert.ThrowsAsync<TimeoutException>(() => listener.EnsureReceived<TestMessage>(x => x.Id == "001", timeout));
        }

        [Test]
        public void EnsureReceived_provides_expression_details_on_timeout()
        {
            using var listener = MessageListener.Start(_source);
            var timeout = TimeSpan.FromMilliseconds(100);
            var ex = Assert.ThrowsAsync<TimeoutException>(
                () => listener.EnsureReceived<TestMessage>(x => x.Id == "001", timeout));
            Assert.That(ex.Message, Is.EqualTo($"Failed to receive {nameof(TestMessage)} within 100ms: No message received matching criteria: x => (x.Id == \"001\")"));
        }

        [Test]
        public void EnsureReceived_provides_custom_error_details_on_timeout()
        {
            using var listener = MessageListener.Start(_source);
            var timeout = TimeSpan.FromMilliseconds(100);
            var ex = Assert.ThrowsAsync<TimeoutException>(
                () => listener.EnsureReceived<TestMessage>(x => x.Id == "001", "Test Message 001 not received", timeout));
            Assert.That(ex.Message, Is.EqualTo($"Failed to receive {nameof(TestMessage)} within 100ms: Test Message 001 not received"));
        }

        [Test]
        public void EnsureReceived_provides_message_details_on_predicate_failure_when_awaiting_new_message()
        {
            using var listener = MessageListener.Start(_source);
            var receiveTask = listener.EnsureReceived<TestMessage>(m => m.Text.Length > 0);
            var msg = new TestMessage("000") { Text = null };
            _source.Publish(msg);
            var ex = Assert.ThrowsAsync<MessagePredicateEvaluationException>(() => receiveTask);
            Assert.That(ex.Message, Is.EqualTo($"Unable to evaluate predicate on message {nameof(TestMessage)}: Object reference not set to an instance of an object."));
            Assert.That(ex.MessageObject, Is.SameAs(msg));
        }

        [Test]
        public void EnsureReceived_provides_message_details_on_predicate_failure_when_awaiting_already_received_message()
        {
            using var listener = MessageListener.Start(_source);
            var msg = new TestMessage("000") { Text = null };
            _source.Publish(msg);

            var ex = Assert.ThrowsAsync<MessagePredicateEvaluationException>(() => listener.EnsureReceived<TestMessage>(m => m.Text.Length > 0));
            Assert.That(ex.Message, Is.EqualTo($"Unable to evaluate predicate on message {nameof(TestMessage)}: Object reference not set to an instance of an object."));
            Assert.That(ex.MessageObject, Is.SameAs(msg));
        }

        [Test]
        public void EnsureReceived_throws_OperationCanceledException_when_cancellation_token_is_cancelled()
        {
            using var listener = MessageListener.Start(_source);

            Assert.ThrowsAsync<OperationCanceledException>(() =>
                listener.EnsureReceived<TestMessage>(m => m.Id != null,
                    cancellationToken: new CancellationTokenSource(100).Token));

            var ex = Assert.ThrowsAsync<OperationCanceledException>(() =>
                  listener.EnsureReceived<TestMessage>(m => m.Id != null, "Error message",
                      cancellationToken: new CancellationTokenSource(100).Token));
        }

        [Test]
        public void EnsureReceived_throws_OperationCanceledException_when_listener_is_disposed()
        {
            var listener = MessageListener.Start(_source);

            var receiveTask = listener.EnsureReceived<TestMessage>(m => m.Id != null, "Error message");
            listener.Dispose();
            Assert.ThrowsAsync<OperationCanceledException>(() => receiveTask);
        }
    }
}