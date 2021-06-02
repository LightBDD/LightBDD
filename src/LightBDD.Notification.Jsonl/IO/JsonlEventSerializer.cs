using System.Text.Json;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.Implementation;

namespace LightBDD.Notification.Jsonl.IO
{
    /// <summary>
    /// JsonlEventSerializer allowing to serialize and deserialize the notification events.
    /// </summary>
    public class JsonlEventSerializer
    {
        private static readonly JsonSerializerOptions JsonlOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IgnoreNullValues = true,
            WriteIndented = false
        };

        /// <summary>
        /// Serializes the notification event.
        /// </summary>
        public string Serialize(NotificationEvent e) => JsonSerializer.Serialize(e, e.GetType(), JsonlOptions);

        /// <summary>
        /// Deserializes the notification event, returning the specific event instance or <seealso cref="UnknownEvent"/> type.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public NotificationEvent Deserialize(string line)
        {
            using var doc = JsonDocument.Parse(line);
            var type = doc.RootElement.GetProperty("_c").GetString();
            return EventMapper.TryGetType(type, out var t)
                ? (NotificationEvent)JsonSerializer.Deserialize(line, t, JsonlOptions)
                : new UnknownEvent(type);
        }
    }
}