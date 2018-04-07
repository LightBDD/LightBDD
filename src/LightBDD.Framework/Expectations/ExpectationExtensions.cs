using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    public static class ExpectationExtensions
    {
        public static IExpectationComposer<T> Or<T>(this Expectation<T> expectation)
        {
            return new OrComposer<T>(expectation);
        }

        public static IExpectationComposer<T> And<T>(this Expectation<T> expectation)
        {
            return new AndComposer<T>(expectation);
        }
    }
}