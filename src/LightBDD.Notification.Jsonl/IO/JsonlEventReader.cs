using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.Implementation;

namespace LightBDD.Notification.Jsonl.IO
{
    /// <summary>
    /// Jsonl event reader.
    /// </summary>
    public class JsonlEventReader
    {
        private static readonly JsonSerializerOptions JsonlOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IgnoreNullValues = true
        };

        private readonly StreamReader _reader;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonlEventReader(Stream uft8Stream)
        {
            _reader = new StreamReader(uft8Stream);
        }

        /// <summary>
        /// Reads all events from the stream.
        /// </summary>
        public async IAsyncEnumerable<ProgressEvent> ReadAll([EnumeratorCancellation] CancellationToken cancellationToken = default)
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

        private async Task<ProgressEvent> ReadNext(CancellationToken cancellationToken)
        {
            var line = await _reader.ReadLineAsync();
            if (line == null)
                return null;
            line = line.TrimEnd(',');

            using (var doc = JsonDocument.Parse(line, new JsonDocumentOptions { AllowTrailingCommas = true }))
            {
                var code = doc.RootElement.GetProperty("_c").GetString();

                return EventMapper.TryGetType(code, out var t)
                    ? (ProgressEvent)JsonSerializer.Deserialize(line, t, JsonlOptions)
                    : new UnknownEvent(code);
            }
        }
    }
}