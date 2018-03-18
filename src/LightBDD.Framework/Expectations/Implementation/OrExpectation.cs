using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class OrExpectation<T> : IExpectation<T>
    {
        private readonly IExpectation<T> _left;
        private readonly IExpectation<T> _right;

        public OrExpectation(IExpectation<T> left, IExpectation<T> right)
        {
            _left = left;
            _right = right;
        }

        public string Description => _left.Description + " or " + _right.Description;
        public bool IsValid(T value)
        {
            return _left.IsValid(value) || _right.IsValid(value);
        }

        public string Format(IValueFormattingService formattingService)
        {
            return _left.Format(formattingService) + " or " + _right.Format(formattingService);
        }
    }
}