using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Converters;

namespace LightBDD.Notification.Jsonl.Events
{
    public abstract class Event
    {
        [JsonPropertyName("_c")]
        public virtual string TypeCode => GetType().Name;

        [JsonPropertyName("_t")]
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Time { get; set; }
    }
}