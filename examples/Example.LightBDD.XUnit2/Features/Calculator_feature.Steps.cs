using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.XUnit2;
using Xunit;
using Xunit.Abstractions;

namespace Example.LightBDD.XUnit2.Features
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
            Assert.Equal(result, _calculator.Add(x, y));
        }

        private void Then_divinding_X_by_Y_should_give_RESULT(int x, int y, int result)
        {
            Assert.Equal(result, _calculator.Divide(x, y));
        }

        private void Then_multiplying_X_by_Y_should_give_RESULT(int x, int y, int result)
        {
            if (x < 0 || y < 0)
                StepExecution.Current.IgnoreScenario("Negative numbers are not supported yet");
            Assert.Equal(result, _calculator.Multiply(x, y));
        }

        public Calculator_feature(ITestOutputHelper output) : base(output)
        {
        }
    }
}