using System.Linq;
using LightBDD.Framework.Messaging;
using LightBDD.Framework.UnitTests.Messaging.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Messaging
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class MessageListener_GetMessages_tests
    {
        private readonly MessageSource _source = new MessageSource();

        [Test]
        public void GetMessages_Of_Object_should_return_all_received_messages()
        {
            var msg1 = 55;
            var msg2 = "string";
            var msg3 = new TestMessage("000");
            using var listener = MessageListener.Start(_source);
            _source.Publish(msg1);
            _source.Publish(msg2);
            _source.Publish(msg3);
            Assert.That(listener.GetMessages<object>(), Is.EquivalentTo(new object[] { msg1, msg2, msg3 }));
        }

        [Test]
        public void GetMessages_Of_T_should_return_only_messages_matching_specific_type()
        {
            var msg1 = 55;
            var msg2 = "string";
            var msg3 = new TestMessage("000");
            var msg4 = new TestMessage("001");
            var msg5 = new DerivedTestMessage("002");
            using var listener = MessageListener.Start(_source);
            _source.Publish(msg1);
            _source.Publish(msg2);
            _source.Publish(msg3);
            _source.Publish(msg4);
            _source.Publish(msg5);
            Assert.That(listener.GetMessages<TestMessage>(), Is.EquivalentTo(new object[] { msg3, msg4, msg5 }));
        }

        [Test]
        public void GetMessages_Of_T_should_messages_in_order_from_latest_to_oldest()
        {
            var msg1 = new TestMessage("001");
            var msg2 = new TestMessage("002");
            var msg3 = new TestMessage("003");
            using var listener = MessageListener.Start(_source);
            _source.Publish(msg1);
            _source.Publish(msg2);
            _source.Publish(msg3);
            Assert.That(listener.GetMessages<TestMessage>().ToArray(),
                Is.EqualTo(new[] { msg3, msg2, msg1 }));
        }
    }
}