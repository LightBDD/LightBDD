using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    public sealed class ScenarioDiscovered : Event
    {
        [JsonPropertyName("i")]
        public Guid Id { get; set; }
        [JsonPropertyName("n")]
        public NameModel Name { get; set; }
        [JsonPropertyName("p")]
        public Guid ParentId { get; set; }
        [JsonPropertyName("l")]
        public string[] Labels { get; set; }
        [JsonPropertyName("c")]
        public string[] Categories { get; set; }
    }
}