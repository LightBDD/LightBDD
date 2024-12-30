using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation;

internal class TypeExpectation<T> : Expectation<object>
{
    public static readonly TypeExpectation<T> Instance = new();

    public override ExpectationResult Verify(object value, IValueFormattingService formattingService)
    {
        var actualType = value?.GetType();

        if (actualType == typeof(T))
            return ExpectationResult.Success;

        return FormatFailure(formattingService, $"got type '{actualType?.Name ?? formattingService.Symbols.NullValue}'");
    }

    public override string Format(IValueFormattingService formattingService) => $"be of type '{typeof(T).Name}'";
}