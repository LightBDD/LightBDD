using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Notification.Jsonl
{
    public class JsonlProgressNotificationWriter
    {
        private static readonly byte[] LineSeparator = Encoding.UTF8.GetBytes(",\n");
        private readonly Stream _utf8Stream;
        private static readonly JsonSerializerOptions JsonlOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IgnoreNullValues = true,
            WriteIndented = false
        };

        public JsonlProgressNotificationWriter(Stream utf8Stream)
        {
            _utf8Stream = utf8Stream;
        }

        public async Task Write(Event e, CancellationToken cancellationToken = default)
        {
            await JsonSerializer.SerializeAsync(_utf8Stream, e, e.GetType(), JsonlOptions, cancellationToken);
            await _utf8Stream.WriteAsync(LineSeparator, 0, LineSeparator.Length, cancellationToken);
        }
    }
}
