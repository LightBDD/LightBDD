namespace LightBDD.Framework.Expectations
{

    public interface IExpectationComposer
    {
        IExpectationComposer Not { get; }
        Expectation<T> Compose<T>(Expectation<T> expectation);
    }
}