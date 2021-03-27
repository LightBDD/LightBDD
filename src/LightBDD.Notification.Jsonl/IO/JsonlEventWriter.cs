using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Notification.Jsonl.IO
{
    /// <summary>
    /// Jsonl event writer.
    /// </summary>
    public class JsonlEventWriter
    {
        private static readonly byte[] LineSeparator = Encoding.UTF8.GetBytes(",\n");
        private readonly Stream _utf8Stream;
        private static readonly JsonSerializerOptions JsonlOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IgnoreNullValues = true,
            WriteIndented = false
        };

        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonlEventWriter(Stream utf8Stream)
        {
            _utf8Stream = utf8Stream;
        }

        /// <summary>
        /// Writes event to the stream.
        /// </summary>
        public async Task Write(ProgressEvent e, CancellationToken cancellationToken = default)
        {
            await JsonSerializer.SerializeAsync(_utf8Stream, e, e.GetType(), JsonlOptions, cancellationToken);
            await _utf8Stream.WriteAsync(LineSeparator, 0, LineSeparator.Length, cancellationToken);
        }
    }
}
