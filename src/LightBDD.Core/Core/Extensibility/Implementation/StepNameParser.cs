using System;
using System.Linq;
using System.Text;
using LightBDD.Core.Formatting;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class StepNameParser
    {
        private readonly INameFormatter _nameFormatter;

        public StepNameParser(INameFormatter nameFormatter)
        {
            _nameFormatter = nameFormatter;
        }

        public string GetStepNameFormat(string stepRawName, ParameterDescriptor[] stepParameters)
        {
            var name = _nameFormatter.FormatName(stepRawName);
            var sb = new StringBuilder();

            var replacements = stepParameters
                .Select((param, index) => ToArgumentReplacement(name, param.RawName, index))
                .OrderBy(r => r.Position)
                .ToArray();
            int lastPos = 0;
            foreach (var replacement in replacements)
            {
                if (lastPos < replacement.Position)
                    sb.Append(name.Substring(lastPos, replacement.Position - lastPos));
                sb.Append(replacement.Value);
                lastPos = replacement.Position + replacement.CharactersToReplace;
            }

            sb.Append(name.Substring(lastPos));
            return sb.ToString();
        }

        private static ArgumentReplacement ToArgumentReplacement(string stepName, string parameterName, int argumentIndex)
        {
            int position = FindArgument(stepName, parameterName.ToUpperInvariant(), StringComparison.Ordinal);
            if (position >= 0)
                return new ArgumentReplacement(position, string.Format("\"{{{0}}}\"", argumentIndex), parameterName.Length);
            position = FindArgument(stepName, parameterName, StringComparison.OrdinalIgnoreCase);
            if (position >= 0)
                return new ArgumentReplacement(position + parameterName.Length, string.Format(" \"{{{0}}}\"", argumentIndex), 0);
            return new ArgumentReplacement(stepName.Length, string.Format(" [{0}: \"{{{1}}}\"]", parameterName, argumentIndex), 0);
        }

        private static int FindArgument(string name, string argument, StringComparison stringComparison)
        {
            int pos = 0;
            while ((pos = name.IndexOf(argument, pos, stringComparison)) >= 0)
            {
                if ((pos == 0 || name[pos - 1] == ' ') &&
                    (pos + argument.Length == name.Length || name[pos + argument.Length] == ' '))
                    return pos;
                pos += argument.Length;
            }
            return -1;
        }

        private class ArgumentReplacement
        {
            public ArgumentReplacement(int position, string value, int charactersToReplace)
            {
                Position = position;
                Value = value;
                CharactersToReplace = charactersToReplace;
            }

            public int Position { get; }
            public string Value { get; }
            public int CharactersToReplace { get; }
        }
    }
}