using System;
using LightBDD.Core.Formatting.Parameters;

namespace LightBDD.Framework.Expectations
{
    public class ExpectedValue<T>:IVerifiableParameter
    {
        private bool? _isEqual;
        public T Expected { get; }
        public T Actual { get; private set; }
        public bool IsValid => _isEqual.GetValueOrDefault(false);

        private ExpectedValue(T expected)
        {
            Expected = expected;
        }

        public ExpectedValue<T> SetActual(T actual)
            => SetActual(actual, AreEqual);

        private ExpectedValue<T> SetActual(T actual, Func<T, T, bool> isEqual)
        {
            Actual = actual;
            _isEqual = isEqual(Expected, Actual);
            return this;
        }

        private static bool AreEqual(T expected, T actual)
        {
            return expected is IEquatable<T> equatable
                ? equatable.Equals(actual)
                : Equals(expected, actual);
        }

        public static implicit operator ExpectedValue<T>(T expected) => new ExpectedValue<T>(expected);

        public override string ToString()
        {
            var expected = Format(Expected);
            var actual = _isEqual.HasValue ? Format(Actual) : "<?>";
            if (!IsValid)
                return $"{expected} (but was {actual})";
            if (expected == actual)
                return actual;
            return $"{expected} (matches {actual})";
        }

        private string Format(T value)
        {
            return $"{value}";
        }
    }
}
