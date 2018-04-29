using System;

namespace LightBDD.Framework.Expectations
{

    public interface IExpectationComposer
    {
        IExpectationComposer Not { get; }
        Expectation<T> Compose<T>(Expectation<T> expectation);
        [Obsolete("This is not a valid expectation method", true)]
        bool Equals(object x);
    }
}