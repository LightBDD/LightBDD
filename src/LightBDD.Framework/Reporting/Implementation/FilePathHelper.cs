using System;
using System.IO;
using System.Linq;

namespace LightBDD.Framework.Reporting.Implementation
{
    internal class FilePathHelper
    {
        private static readonly char[] Separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        internal static string ResolveAbsolutePath(string outputPath)
        {
            if (outputPath.StartsWith("~"))
                return AppContext.BaseDirectory.TrimEnd(Separators) + Path.DirectorySeparatorChar + outputPath.Substring(1).TrimStart(Separators);

            if (IsUnc(outputPath))
                return outputPath;

            if (IsStartingWithDirSeparator(outputPath))
                return CombineRootedPathWithCurrentRoot(outputPath);

            return Path.Combine(Directory.GetCurrentDirectory(), outputPath);
        }

        private static string CombineRootedPathWithCurrentRoot(string rootedOutputPath)
        {
            var workingRoot = Path.GetPathRoot(Directory.GetCurrentDirectory());
            return Path.Combine(workingRoot, rootedOutputPath.TrimStart(Separators));
        }

        private static bool IsStartingWithDirSeparator(string outputPath)
        {
            return outputPath.Length > 0 && Separators.Any(s => outputPath[0] == s);
        }

        private static bool IsUnc(string outputPath)
        {
            return outputPath.StartsWith("\\\\");
        }

        internal static void EnsureOutputDirectoryExists(string outputPath)
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (directory != null)
                Directory.CreateDirectory(directory);
        }
    }
}