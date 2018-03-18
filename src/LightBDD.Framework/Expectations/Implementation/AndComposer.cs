namespace LightBDD.Framework.Expectations.Implementation
{
    internal class AndComposer<T> : LogicalComposer<T>
    {
        public AndComposer(IExpectation<T> left) : base(left)
        {
        }

        protected override IExpectation<T> CreateExpectation(IExpectation<T> left, IExpectation<T> right)
        {
            return new AndExpectation<T>(left, right);
        }
    }
}