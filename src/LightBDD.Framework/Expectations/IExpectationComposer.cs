namespace LightBDD.Framework.Expectations
{
    public interface IExpectationComposer
    {
        IExpectationComposer Not { get; }
        IExpectationComposer<T> For<T>();
    }

    public interface IExpectationComposer<T>
    {
        IExpectationComposer<T> Not { get; }
        Expectation<T> Compose(Expectation<T> expectation);
    }
}