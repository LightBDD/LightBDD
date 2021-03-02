using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    public class NameModel
    {
        [JsonPropertyName("p")]
        public NameParameterModel[] Parameters { get; set; } = Array.Empty<NameParameterModel>();

        [JsonPropertyName("f")]
        public string Format { get; set; }
    }
}