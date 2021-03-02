using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    public class StepNameModel : NameModel
    {
        [JsonPropertyName("o")]
        public string OriginalTypeName { get; set; }
        [JsonPropertyName("t")]
        public string TypeName { get; set; }
    }
}