using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Notification.Jsonl
{
    public class JsonlProgressNotificationReader
    {
        private static readonly Dictionary<string, Type> Mapping = typeof(JsonlProgressNotificationReader).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(Event).IsAssignableFrom(t)).ToDictionary(t => t.Name);

        private static readonly JsonSerializerOptions JsonlOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IgnoreNullValues = true
        };

        private readonly StreamReader _reader;

        public JsonlProgressNotificationReader(Stream uft8Stream)
        {
            _reader = new StreamReader(uft8Stream);
        }

        public async IAsyncEnumerable<Events.Event> ReadAll([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var e = await ReadNext(cancellationToken);

                if (e == null)
                    yield break;
                yield return e;
            }
        }

        private async Task<Event> ReadNext(CancellationToken cancellationToken)
        {
            var line = await _reader.ReadLineAsync();
            if (line == null)
                return null;
            line = line.TrimEnd(',');

            using (var doc = JsonDocument.Parse(line, new JsonDocumentOptions { AllowTrailingCommas = true }))
            {
                var type = doc.RootElement.GetProperty("_c").GetString();

                return Mapping.TryGetValue(type, out var t)
                    ? (Event)JsonSerializer.Deserialize(line, t, JsonlOptions)
                    : new Unknown(type);
            }
        }
    }
}