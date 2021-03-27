using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    /// <summary>
    /// Step name model.
    /// </summary>
    public class StepNameModel : NameModel
    {
        /// <summary>
        /// Original step type name.
        /// </summary>
        [JsonPropertyName("o")]
        public string OriginalTypeName { get; set; }

        /// <summary>
        /// Step type name.
        /// </summary>
        [JsonPropertyName("t")]
        public string TypeName { get; set; }
    }
}