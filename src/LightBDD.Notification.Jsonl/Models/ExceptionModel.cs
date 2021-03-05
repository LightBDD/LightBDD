using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    public class ExceptionModel
    {
        [JsonPropertyName("i")]
        public ExceptionModel InnerException { get; set; }
        [JsonPropertyName("s")]
        public string StackTrace { get; set; }
        [JsonPropertyName("t")]
        public string Type { get; set; }
        [JsonPropertyName("m")]
        public string Message { get; set; }
    }
}