using Example.Domain.Domain;
using LightBDD.NUnit2;
using NUnit.Framework;

namespace Example.LightBDD.NUnit2.Features
{
    public partial class Calculator_feature : FeatureFixture
    {
        private Calculator _calculator;

        private void Given_a_calculator()
        {
            _calculator = new Calculator();
        }

        private void Then_adding_X_to_Y_should_give_RESULT(int x, int y, int result)
        {
            Assert.AreEqual(result, (int) _calculator.Add(x, y));
        }

        private void Then_dividing_X_by_Y_should_give_RESULT(int x, int y, int result)
        {
            Assert.AreEqual(result, (int) _calculator.Divide(x, y));
        }

        private void Then_multiplying_X_by_Y_should_give_RESULT(int x, int y, int result)
        {
            if (x < 0 || y < 0)
                Assert.Inconclusive("Negative numbers are not supported yet");
            Assert.AreEqual(result, (int) _calculator.Multiply(x, y));
        }
    }
}