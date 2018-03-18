using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations
{
    public interface IExpectation<T> : ISelfFormattable
    {
        string Description { get; }
        bool IsValid(T value);
    }
}