using System;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class StepTypeProcessor
    {
        private readonly INameFormatter _nameFormatter;
        private readonly StepTypeConfiguration _configuration;

        public StepTypeProcessor(INameFormatter nameFormatter, StepTypeConfiguration configuration)
        {
            _nameFormatter = nameFormatter;
            _configuration = configuration;
        }

        public IStepTypeNameInfo GetStepTypeName(string rawStepTypeName, ref string formattedRawName, string previousStepTypeName)
        {
            var stepTypeName = string.IsNullOrWhiteSpace(rawStepTypeName)
                ? ExtractStepTypeFromFormattedName(ref formattedRawName)
                : _nameFormatter.FormatName(rawStepTypeName);

            return string.IsNullOrWhiteSpace(stepTypeName) 
                ? null 
                : new StepTypeNameInfo(FormatStepTypeName(NormalizeStepTypeName(stepTypeName, previousStepTypeName)), FormatStepTypeName(stepTypeName));
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