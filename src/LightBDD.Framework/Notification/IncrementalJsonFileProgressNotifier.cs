using System;
using System.IO;
using System.Threading.Tasks;
using LightBDD.Framework.Reporting.Implementation;

namespace LightBDD.Framework.Notification
{
    public class IncrementalJsonFileProgressNotifier : IncrementalJsonProgressNotifier
    {
        private readonly StreamWriter _fileWriter;

        public IncrementalJsonFileProgressNotifier(string outputPath)
        {
            var filePath = FilePathHelper.ResolveAbsolutePath(outputPath);
            FilePathHelper.EnsureOutputDirectoryExists(filePath);
            _fileWriter = new StreamWriter(File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        protected override void WriteMessage(Action<StreamWriter> writeFn)
        {
            writeFn(_fileWriter);
            _fileWriter.Flush();
        }

        protected override void OnFinish()
        {
            _fileWriter.Dispose();
        }
    }
}