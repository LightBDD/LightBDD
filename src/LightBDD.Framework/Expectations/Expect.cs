using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    public static class Expect
    {
        public static IExpectationComposer To => new ExpectationComposer();
    }
}