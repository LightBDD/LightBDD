namespace LightBDD.Framework.UnitTests.Expectations.Helpers
{
    public interface IExpectationScenario
    {
        void AssertFormat();
        void AssertExpectationMatchingValues();
        void AssertExpectationNotMatchingValues();
        void AssertNegatedExpectationNotMatchingValues();
        void AssertNegatedExpectationMatchingValues();
    }
}