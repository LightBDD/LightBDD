using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.IO;
using NUnit.Framework;
using RandomTestValues;
using Shouldly;

namespace LightBDD.Notification.Jsonl.UnitTests
{
    [TestFixture]
    public class JsonEventSerializerTests
    {
        JsonlEventSerializer _serializer = new JsonlEventSerializer();
        [Test]
        public void Serializer_should_deserialize_events()
        {
            var e1 = RandomValue.Object<TestExecutionStartingEvent>();
            var e2 = RandomValue.Object<TestExecutionFinishedEvent>();
            var lines = WriteEvents(e1, e2);
            var actualEvents = lines.Select(_serializer.Deserialize).ToArray();

            actualEvents.ShouldBeEquivalentTo(new NotificationEvent[] { e1, e2 });
        }

        [Test]
        public void Serializer_should_return_unknown_type_for_unrecognized_events()
        {
            var input = new[]
            {
                @"{""_c"":""TestExecutionStarting"",""_t"":21288995999998785}",
                @"{""_c"":""something"",""_t"":97551612000000012}",
                @"{""_c"":""TestExecutionFinished"",""_t"":97551612000000012}"
            };
            var actualEvents = input.Select(_serializer.Deserialize).ToArray();

            actualEvents.Select(e => e.GetType()).ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    typeof(TestExecutionStartingEvent),
                    typeof(UnknownEvent),
                    typeof(TestExecutionFinishedEvent)
                });
            actualEvents[1].TypeCode.ShouldBe("something");
        }

        [Test]
        [TestCaseSource(nameof(GetAllEventTypes))]
        public void Serializer_should_reconstruct_the_event(Type eventType)
        {
            var e = CreateEvent(eventType);
            var lines = WriteEvents(e);
            var actualEvents = lines.Select(_serializer.Deserialize).ToArray();
            actualEvents.Single().ShouldBeEquivalentTo(e);
        }

        private static IEnumerable<Type> GetAllEventTypes() => typeof(NotificationEvent).Assembly.GetTypes().Where(t =>
            typeof(NotificationEvent).IsAssignableFrom(t) && t != typeof(NotificationEvent) && t != typeof(UnknownEvent));

        private string[] WriteEvents(params NotificationEvent[] events)
        {
            return events.Select(_serializer.Serialize).ToArray();
        }

        private static NotificationEvent CreateEvent(Type eventType)
        {
            Func<RandomValueSettings, object> fn = RandomValue.Object<object>;
            var genericMethod = fn.Method.GetGenericMethodDefinition().MakeGenericMethod(eventType);
            return (NotificationEvent)genericMethod.Invoke(null, new object[] { new RandomValueSettings(2) });
        }
    }
}
