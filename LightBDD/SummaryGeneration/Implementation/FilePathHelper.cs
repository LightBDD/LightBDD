using System;
using System.IO;

namespace LightBDD.SummaryGeneration.Implementation
{
    internal class FilePathHelper
    {
        internal static string ResolveAbsolutePath(string outputPath)
        {
            if (outputPath.StartsWith("~"))
                return AppDomain.CurrentDomain.BaseDirectory + "\\" + outputPath.Substring(1);
            return Path.Combine(Directory.GetCurrentDirectory(), outputPath);
        }

        internal static void EnsureOutputDirectoryExists(string outputPath)
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (directory != null)
                Directory.CreateDirectory(directory);
        }
    }
}