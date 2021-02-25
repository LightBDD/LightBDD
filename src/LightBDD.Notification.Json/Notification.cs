using System;
using System.Text.Json.Serialization;

namespace LightBDD.Notification.Json
{
    public abstract class Notification
    {
        [JsonPropertyName("_t")]
        public ulong Time { get; set; }

        [JsonPropertyName("_i")]
        public Guid RuntimeId { get; set; }

        [JsonPropertyName("_c")]
        public string TypeCode => GetTypeCode();

        protected abstract string GetTypeCode();
    }
}