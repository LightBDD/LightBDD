using System;
using System.IO;

namespace LightBDD.Framework.Reporting.Implementation
{
    internal class FilePathHelper
    {
        internal static string ResolveAbsolutePath(string outputPath)
        {
            if (outputPath.StartsWith("~"))
#if NET45
                return Combine(AppDomain.CurrentDomain.BaseDirectory, outputPath.Substring(1));
#else
                return Combine(AppContext.BaseDirectory, outputPath.Substring(1));
#endif

            return Combine(Directory.GetCurrentDirectory(), outputPath);
        }

        internal static void EnsureOutputDirectoryExists(string outputPath)
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (directory != null)
                Directory.CreateDirectory(directory);
        }

        private static string Combine(string baseDirectory, string subPath)
        {
            return Path.Combine(baseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), subPath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
    }
}