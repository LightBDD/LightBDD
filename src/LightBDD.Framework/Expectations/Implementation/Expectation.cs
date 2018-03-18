using System;
using System.Linq.Expressions;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations.Implementation
{
    internal class Expectation<T> : IExpectation<T>
    {
        public string Description { get; }
        private readonly Func<T, bool> _predicate;

        public Expectation(Expression<Func<T, bool>> predicate)
            : this(predicate.ToString(), predicate.Compile()) { }

        public Expectation(string description, Func<T, bool> predicate)
        {
            Description = description;
            _predicate = predicate;
        }

        public override string ToString()
        {
            return Description;
        }

        public string Format(IValueFormattingService formattingService)
        {
            return Description;
        }


        public bool IsValid(T value) => _predicate.Invoke(value);
    }
}