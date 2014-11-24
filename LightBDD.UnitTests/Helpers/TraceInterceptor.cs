using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LightBDD.UnitTests.Helpers
{
    class TraceInterceptor : TraceListener
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        public TraceInterceptor()
        {
            Trace.Listeners.Add(this);
        }

        protected override void Dispose(bool disposing)
        {
            Trace.Listeners.Remove(this);
        }

        public override void Write(string message)
        {
            _buffer.Append(message);
        }

        public override void WriteLine(string message)
        {
            _buffer.AppendLine(message);
        }

        public string GetCapturedText()
        {
            return _buffer.ToString();
        }
    }
}