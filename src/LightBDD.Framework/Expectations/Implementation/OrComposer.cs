namespace LightBDD.Framework.Expectations.Implementation
{
    internal class OrComposer<T> : LogicalComposer<T>
    {
        public OrComposer(IExpectation<T> left) : base(left)
        {
        }

        protected override IExpectation<T> CreateExpectation(IExpectation<T> left, IExpectation<T> right)
        {
            return new OrExpectation<T>(left, right);
        }
    }
}