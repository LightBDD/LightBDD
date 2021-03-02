using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    public class NameParameterModel
    {

        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }
        [JsonPropertyName("v")]
        public string FormattedValue { get; set; }
        [JsonPropertyName("e")]
        public bool IsEvaluated { get; set; }
    }
}