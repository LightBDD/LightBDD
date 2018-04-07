namespace LightBDD.Framework.Expectations.Implementation
{
    internal class AndComposer<T> : LogicalComposer<T>
    {
        public AndComposer(Expectation<T> left) : base(left)
        {
        }

        protected override Expectation<T> Compose(Expectation<T> left, Expectation<T> right)
        {
            return new AndExpectation<T>(left, right);
        }
    }
}