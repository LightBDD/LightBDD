using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    public static class ExpectedExtensions
    {
        public static IExpectationComposer<T> And<T>(this Expected<T> expected)
        {
            return new AndComposer<T>(expected.Expectation);
        }

        public static IExpectationComposer<T> Or<T>(this Expected<T> expected)
        {
            return new OrComposer<T>(expected.Expectation);
        }
    }
}