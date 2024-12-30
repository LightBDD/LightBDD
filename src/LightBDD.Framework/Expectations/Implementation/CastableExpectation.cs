using System;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation;

internal class CastableExpectation<T> : Expectation<object>
{
    public static readonly CastableExpectation<T> Instance = new();

    public override ExpectationResult Verify(object value, IValueFormattingService formattingService)
    {
        if (CastHelper.IsAssignableTo<T>(value))
            return ExpectationResult.Success;

        if (NumericTypeHelper.IsNumeric(typeof(T)) && NumericTypeHelper.IsNumeric(value))
        {
            try
            {
                if (CastHelper.TryConvertWithoutPrecisionLoss<T>(value, out _))
                    return ExpectationResult.Success;
                return FormatFailure(formattingService, $"got '{value.GetType().Name}' type with value '{formattingService.FormatValue(value)}' which cannot be cast without precision loss");
            }
            catch (Exception ex)
            {
                return FormatFailure(formattingService, $"got '{value.GetType().Name}' type with value '{formattingService.FormatValue(value)}' which cannot be cast: {ex.Message}");
            }
        }

        return FormatFailure(formattingService, $"got '{value?.GetType().Name ?? formattingService.Symbols.NullValue}' type");
    }

    public override string Format(IValueFormattingService formattingService) => $"be castable to '{typeof(T).Name}'";
}