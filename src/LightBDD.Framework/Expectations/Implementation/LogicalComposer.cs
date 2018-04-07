namespace LightBDD.Framework.Expectations.Implementation
{
    internal abstract class LogicalComposer<T> : IExpectationComposer<T>
    {
        private readonly Expectation<T> _left;
        private IExpectationComposer<T> _right = new ExpectationComposer<T>();

        public LogicalComposer(Expectation<T> left)
        {
            _left = left;
        }

        public IExpectationComposer<T> Not
        {
            get
            {
                _right = _right.Not;
                return this;
            }
        }
        public Expectation<T> Compose(Expectation<T> expectation)
        {
            return Compose(_left, _right.Compose(expectation));
        }

        protected abstract Expectation<T> Compose(Expectation<T> left, Expectation<T> right);
    }
}