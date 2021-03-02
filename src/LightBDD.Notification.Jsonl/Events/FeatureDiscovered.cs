using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    public sealed class FeatureDiscovered : Event
    {
        [JsonPropertyName("l")]
        public string[] Labels { get; set; } = Array.Empty<string>();

        [JsonPropertyName("d")]
        public string Description { get; set; }

        [JsonPropertyName("n")]
        public NameModel Name { get; set; }

        [JsonPropertyName("i")]
        public Guid Id { get; set; }
    }
}