namespace LightBDD.Framework.Expectations.Implementation
{
    internal class ExpectationComposer : IExpectationComposer
    {
        private bool _isNot;
        public IExpectationComposer Not
        {
            get
            {
                _isNot = !_isNot;
                return this;
            }
        }

        public Expectation<T> Compose<T>(Expectation<T> expectation)
        {
            return _isNot ? new NotExpectation<T>(expectation) : expectation;
        }
    }
}