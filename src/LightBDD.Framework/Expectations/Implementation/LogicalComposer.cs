namespace LightBDD.Framework.Expectations.Implementation
{
    internal abstract class LogicalComposer<T> : IExpectationComposer<T>
    {
        private readonly IExpectation<T> _left;
        private IExpectationComposer<T> _right = new ExpectationComposer<T>();

        public LogicalComposer(IExpectation<T> left)
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
        public Expected<T> Create(IExpectation<T> expectation)
        {
            var r = _right.Create(expectation);
            return new Expected<T>(CreateExpectation(_left, r.Expectation));
        }

        protected abstract IExpectation<T> CreateExpectation(IExpectation<T> left, IExpectation<T> right);
    }
}