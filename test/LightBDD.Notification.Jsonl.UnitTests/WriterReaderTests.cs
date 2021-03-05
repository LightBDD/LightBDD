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
            var e1 = RandomValue.Object<ExecutionStarting>();
            var e2 = RandomValue.Object<ExecutionFinished>();
            var mem = await WriteEvents(e1, e2);

            var reader = new JsonlEventReader(mem);
            var actualEvents = await reader.ReadAll().ToArrayAsync();

            actualEvents.ShouldBeEquivalentTo(new Event[] { e1, e2 });
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
                    typeof(ExecutionStarting),
                    typeof(Unknown),
                    typeof(ExecutionFinished)
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

        private static IEnumerable<Type> GetAllEventTypes() => typeof(Event).Assembly.GetTypes().Where(t =>
            typeof(Event).IsAssignableFrom(t) && t != typeof(Event) && t != typeof(Unknown));

        private async Task<MemoryStream> WriteEvents(params Event[] events)
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

        private static Event CreateEvent(Type eventType)
        {
            Func<RandomValueSettings, object> fn = RandomValue.Object<object>;
            var genericMethod = fn.Method.GetGenericMethodDefinition().MakeGenericMethod(eventType);
            var e = (Event)genericMethod.Invoke(null, new object[] { new RandomValueSettings(2) });
            return e;
        }
    }
}
