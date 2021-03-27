using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.IO;
using NUnit.Framework;
using RandomTestValues;
using Shouldly;

namespace LightBDD.Notification.Jsonl.UnitTests
{
    [TestFixture]
    public class WriterReaderTests
    {
        [Test]
        public async Task Reader_should_reconstruct_the_event_stream()
        {
            var e1 = RandomValue.Object<ExecutionStartingEvent>();
            var e2 = RandomValue.Object<ExecutionFinishedEvent>();
            var mem = await WriteEvents(e1, e2);

            var reader = new JsonlEventReader(mem);
            var actualEvents = await reader.ReadAll().ToArrayAsync();

            actualEvents.ShouldBeEquivalentTo(new ProgressEvent[] { e1, e2 });
        }

        [Test]
        public async Task Reader_should_return_unknown_type_for_unrecognized_events()
        {
            var input = @"{""_c"":""ExecutionStarting"",""_t"":21288995999998785},
{""_c"":""something"",""_t"":97551612000000012},
{""_c"":""ExecutionFinished"",""_t"":97551612000000012},";
            var mem = new MemoryStream(Encoding.UTF8.GetBytes(input));
            var reader = new JsonlEventReader(mem);

            var actualEvents = await reader.ReadAll().ToArrayAsync();

            actualEvents.Select(e => e.GetType()).ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    typeof(ExecutionStartingEvent),
                    typeof(UnknownEvent),
                    typeof(ExecutionFinishedEvent)
                });
            actualEvents[1].TypeCode.ShouldBe("something");
        }

        [Test]
        [TestCaseSource(nameof(GetAllEventTypes))]
        public async Task Reader_should_reconstruct_the_event(Type eventType)
        {
            var e = CreateEvent(eventType);
            var mem = await WriteEvents(e);

            var reader = new JsonlEventReader(mem);
            var actualEvents = await reader.ReadAll().ToArrayAsync();
            actualEvents.Single().ShouldBeEquivalentTo(e);
        }

        private static IEnumerable<Type> GetAllEventTypes() => typeof(ProgressEvent).Assembly.GetTypes().Where(t =>
            typeof(ProgressEvent).IsAssignableFrom(t) && t != typeof(ProgressEvent) && t != typeof(UnknownEvent));

        private async Task<MemoryStream> WriteEvents(params ProgressEvent[] events)
        {
            var mem = new MemoryStream();
            var writer = new JsonlEventWriter(mem);
            foreach (var e in events)
                await writer.Write(e);

            var str = Encoding.UTF8.GetString(mem.ToArray());
            Console.WriteLine(str);

            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }

        private static ProgressEvent CreateEvent(Type eventType)
        {
            Func<RandomValueSettings, object> fn = RandomValue.Object<object>;
            var genericMethod = fn.Method.GetGenericMethodDefinition().MakeGenericMethod(eventType);
            var e = (ProgressEvent)genericMethod.Invoke(null, new object[] { new RandomValueSettings(2) });
            return e;
        }
    }
}
