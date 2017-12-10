using System;
using System.IO;
using System.Text;

namespace LightBDD.Framework.Notification.Implementation.IncrementalJson
{
    internal class InlineJsonStreamWriter : IDisposable
    {
        private readonly StreamWriter _writer;

        public InlineJsonStreamWriter(Stream stream)
        {
            _writer = new StreamWriter(stream, Encoding.UTF8);
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }

        public InlineJsonStreamWriter WriteText(string name)
        {
            if (name == null)
                _writer.Write("null");
            else
            {
                _writer.Write('"');
                _writer.Write(Escape(name));
                _writer.Write('"');
            }
            return this;
        }

        private string Escape(string name)
        {
            return name
                .Replace("\\", "\\\\")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");
        }

        public InlineJsonStreamWriter WriteDirect(char value)
        {
            _writer.Write(value);
            return this;
        }

        public InlineJsonStreamWriter Write(IJsonItem value)
        {
            value.WriteTo(this);
            return this;
        }

        public InlineJsonStreamWriter Flush()
        {
            _writer.Flush();
            return this;
        }

        public InlineJsonStreamWriter WriteLong(long value)
        {
            _writer.Write(value);
            return this;
        }

        public InlineJsonStreamWriter WriteBoolean(bool value)
        {
            _writer.Write(value);
            return this;
        }

        public InlineJsonStreamWriter WriteDouble(double value)
        {
            _writer.Write(value);
            return this;
        }
    }
}