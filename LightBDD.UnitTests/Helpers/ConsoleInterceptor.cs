using System;
using System.IO;
using System.Text;

namespace LightBDD.UnitTests.Helpers
{
    class ConsoleInterceptor : IDisposable
    {
        private readonly TextWriter _originalOut;
        private readonly StringBuilder _buffer;
        public ConsoleInterceptor()
        {
            _originalOut = Console.Out;
            _buffer = new StringBuilder();
            Console.SetOut(new StringWriter(_buffer));
        }

        public void Dispose()
        {
            Console.SetOut(_originalOut);
        }

        public string GetCapturedText()
        {
            return _buffer.ToString();
        }

        public void Reset()
        {
            _buffer.Clear();
        }
    }
}