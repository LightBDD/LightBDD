using System.Threading.Tasks;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Compact;
using LightBDD.NUnit2;
using NUnit.Framework;

namespace Example.LightBDD.NUnit2.Features
{
    [FeatureDescription(
@"As LightBDD user,
I want to be able to write compact scenarios,
So that I can use LightBDD for more unit-test like tests as well")]
    public class Compact_calculator_scenarios : FeatureFixture
    {
        [Scenario]
        public async Task Adding_numbers()
        {
            Calculator calc = null;
            var result = 0;

            await Runner
                .AddStep("Given calculator", _ => calc = new Calculator())
                .AddStep("When I add two numbers", _ => result = calc.Add(3, 5))
                .AddStep("Then I should get an expected result", _ => Assert.AreEqual(8, result))
                .RunAsync();
        }
    }
}
