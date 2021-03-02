using System;
using System.Text.Json.Serialization;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Notification.Jsonl.Events
{
    public sealed class StepDiscovered : Event
    {
        [JsonPropertyName("i")]
        public Guid Id { get; set; }
        [JsonPropertyName("p")]
        public Guid ParentId { get; set; }
        [JsonPropertyName("n")]
        public StepNameModel Name { get; set; }
        [JsonPropertyName("u")]
        public int Number { get; set; }
        [JsonPropertyName("g")]
        public string GroupPrefix { get; set; }
    }
}