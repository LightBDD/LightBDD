using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations
{
    public interface IExpectation<in T> : ISelfFormattable
    {
        ExpectationResult Verify(T value, IValueFormattingService formattingService);
    }
}