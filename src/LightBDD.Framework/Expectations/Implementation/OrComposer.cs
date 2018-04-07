namespace LightBDD.Framework.Expectations.Implementation
{
    internal class OrComposer<T> : LogicalComposer<T>
    {
        public OrComposer(Expectation<T> left) : base(left)
        {
        }

        protected override Expectation<T> Compose(Expectation<T> left, Expectation<T> right)
        {
            return new OrExpectation<T>(left, right);
        }
    }
}