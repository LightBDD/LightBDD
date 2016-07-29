using System;
using System.Linq;
using LightBDD.Core.Formatting;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class StepTypeProcessor
    {
        private readonly INameFormatter _nameFormatter;
        private readonly StepTypeConfiguration _configuration;

        public StepTypeProcessor(INameFormatter nameFormatter, StepTypeConfiguration configuration)
        {
            _nameFormatter = nameFormatter;
            _configuration = configuration;
        }

        public string GetStepTypeName(string rawStepTypeName, ref string formattedRawName, string lastStepTypeName)
        {
            var stepTypeName = string.IsNullOrWhiteSpace(rawStepTypeName)
                ? ExtractStepTypeFromFormattedName(ref formattedRawName)
                : _nameFormatter.FormatName(rawStepTypeName);
            return FormatStepTypeName(NormalizeStepTypeName(stepTypeName, lastStepTypeName));
        }

        private string ExtractStepTypeFromFormattedName(ref string formattedRawName)
        {
            var name = formattedRawName;

            var type = _configuration.PredefinedStepTypes
                .Where(n =>
                    name.Length > n.Length &&
                    name.StartsWith(n, StringComparison.OrdinalIgnoreCase) &&
                    name[n.Length] == ' ')
                .Select(FormatStepTypeName)
                .FirstOrDefault();

            if (type == null || string.IsNullOrWhiteSpace(name = name.Substring(type.Length + 1)))
                return string.Empty;

            formattedRawName = name;
            return type;
        }

        private string FormatStepTypeName(string stepTypeName)
        {
            return string.IsNullOrWhiteSpace(stepTypeName) ? null : stepTypeName.ToUpperInvariant().Trim();
        }

        private string NormalizeStepTypeName(string stepTypeName, string lastStepTypeName)
        {
            return (!string.IsNullOrEmpty(stepTypeName) && !string.IsNullOrWhiteSpace(_configuration.RepeatedStepReplacement) && string.Equals(stepTypeName, lastStepTypeName, StringComparison.OrdinalIgnoreCase))
                ? FormatStepTypeName(_configuration.RepeatedStepReplacement)
                : stepTypeName;
        }
    }
}