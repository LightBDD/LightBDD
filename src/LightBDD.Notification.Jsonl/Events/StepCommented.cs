using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Events
{
    public sealed class StepCommented : Event
    {
        [JsonPropertyName("i")]
        public Guid StepId { get; set; }
        [JsonPropertyName("c")]
        public string Comment { get; set; }
    }
}