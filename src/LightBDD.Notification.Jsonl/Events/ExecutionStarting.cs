using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    public sealed class ExecutionStarting : Event
    {
        [JsonPropertyName("s")]
        public DateTimeOffset Start { get; set; }
    }
}