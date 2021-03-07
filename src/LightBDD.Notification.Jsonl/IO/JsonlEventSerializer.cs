using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Notification.Jsonl.IO
{
    public class JsonlEventSerializer
    {
        private static readonly Dictionary<string, Type> Mapping = typeof(JsonlEventReader).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(Event).IsAssignableFrom(t)).ToDictionary(t => t.Name);

        private static readonly JsonSerializerOptions JsonlOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IgnoreNullValues = true,
            WriteIndented = false
        };

        public string Serialize(Event e) => JsonSerializer.Serialize(e, e.GetType(), JsonlOptions);

        public Event Deserialize(string line)
        {
            using var doc = JsonDocument.Parse(line);
            var type = doc.RootElement.GetProperty("_c").GetString();
            return Mapping.TryGetValue(type, out var t)
                ? (Event)JsonSerializer.Deserialize(line, t, JsonlOptions)
                : new Unknown(type);
        }
    }
}