using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace LightBDD.Notification.Json
{
    public class NotificationWriter : IDisposable
    {
        private readonly Utf8JsonWriter _writer;
        private static readonly JsonWriterOptions Options = new JsonWriterOptions { Indented = false, SkipValidation = true, Encoder = JavaScriptEncoder.Default };
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        public NotificationWriter(Stream stream)
        {
            _writer = new Utf8JsonWriter(stream, Options);
            _writer.WriteStartArray();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Write(Notification notification) => JsonSerializer.Serialize(_writer, notification, notification.GetType(), SerializerOptions);

        public void Dispose()
        {
            _writer.WriteEndArray();
            _writer.Dispose();
        }
    }

    public class NotificationReader : IDisposable
    {
        public NotificationReader(Stream stream)
        {
           new Utf8JsonReader()
        }

        public void Dispose()
        {
        }
    }
}
