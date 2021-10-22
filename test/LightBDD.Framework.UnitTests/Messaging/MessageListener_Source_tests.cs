using System;
using System.Linq;
using LightBDD.Framework.Messaging;
using LightBDD.Framework.UnitTests.Messaging.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Messaging
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class MessageListener_Source_tests
    {
        private readonly MessageSource _source = new MessageSource();

        [Test]
        public void Dispose_should_deregister_the_listener_from_OnMessage_event()
        {
            MessageListener.Start(_source).Dispose();
            Assert.IsEmpty(_source.GetInvocationList());
        }

        [Test]
        public void Start_should_register_for_OnMessage_event()
        {
            using var listener = MessageListener.Start(_source);
            Assert.IsNotEmpty(_source.GetInvocationList());
        }

        [Test]
        public void It_should_capture_messages_after_start()
        {
            var messages = new[]
            {
                new TestMessage("123"),
                new TestMessage("456"),
                new TestMessage("789")
            };

            _source.Publish(new TestMessage("000"));
            using var listener = MessageListener.Start(_source);

            foreach (var msg in messages)
                _source.Publish(msg);

            Assert.That(listener.GetMessages<TestMessage>(), Is.EquivalentTo(messages));
        }

        [Test]
        public void All_registered_listeners_should_receive_messages_from_source()
        {
            var listeners = Enumerable
                .Range(0, 100)
                .Select(_ => MessageListener.Start(_source))
                .ToArray();

            var msg = new TestMessage("abc");
            _source.Publish(msg);

            for (var index = 0; index < listeners.Length; index++)
            {
                var listener = listeners[index];
                Assert.That(listener.GetMessages<TestMessage>(), Is.EqualTo(new[] { msg }), () => $"Listener {index} did not receive message");
            }
        }

        [Test]
        public void Dispose_of_listener_should_not_interfere_others()
        {
            var listeners = Enumerable
                .Range(0, 100)
                .Select(_ => MessageListener.Start(_source))
                .ToArray();

            int count = 0;
            foreach (var listener in listeners)
            {
                _source.Publish(new TestMessage("id"));
                count++;

                Assert.That(listener.GetMessages<TestMessage>().Count(), Is.EqualTo(count));
                listener.Dispose();
            }
        }

        [Test]
        public void Dispose_of_listener_clears_the_received_message_collection()
        {
            var listener = MessageListener.Start(_source);
            _source.Publish(new TestMessage("0"));
            listener.Dispose();
            Assert.That(listener.GetMessages<TestMessage>(), Is.Empty);
        }
    }
}
