using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Jsonl.Models
{
    /// <summary>
    /// Name model.
    /// </summary>
    public class NameModel
    {
        /// <summary>
        /// Parameters.
        /// </summary>
        [JsonPropertyName("p")]
        public NameParameterModel[] Parameters { get; set; } = Array.Empty<NameParameterModel>();

        /// <summary>
        /// Format.
        /// </summary>
        [JsonPropertyName("f")]
        public string Format { get; set; }
    }
}