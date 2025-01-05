using System;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Metadata;

namespace LightBDD.Framework.Reporting.Formatters;

internal class MarkdownStepNameDecorator : IStepNameDecorator
{
    public static readonly MarkdownStepNameDecorator Instance = new();
    public string DecorateStepTypeName(IStepTypeNameInfo stepTypeName)
    {
        return stepTypeName.Name;
    }

    public string DecorateParameterValue(INameParameterInfo parameter)
    {
        return $"`{parameter.FormattedValue}`{GetVerificationStatus(parameter.VerificationStatus)}";
    }

    public string DecorateNameFormat(string nameFormat)
    {
        return nameFormat;
    }

    public string GetVerificationStatus(ParameterVerificationStatus status)
    {
        return status switch
        {
            ParameterVerificationStatus.NotApplicable => "",
            ParameterVerificationStatus.Success => "☑",
            ParameterVerificationStatus.Failure => "❗",
            ParameterVerificationStatus.Exception => "❗",
            ParameterVerificationStatus.NotProvided => "⚠",
            _ => ""
        };
    }
}