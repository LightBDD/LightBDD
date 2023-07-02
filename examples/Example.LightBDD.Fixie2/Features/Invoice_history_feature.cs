using System.Threading.Tasks;
using LightBDD.Fixie3;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;

namespace Example.LightBDD.Fixie3.Features
{
    /// <summary>
    /// This feature class presents usage of fluent scenario syntax, that is enabled by NewScenario() method.
    /// While this syntax is less readable as it counterparts, it offers a flexibility in mixing various
    /// types of steps and syntax used to compose them, and allows programmatic creation of scenarios.
    /// 
    /// More information on fluent syntax can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#fluent-scenarios
    /// </summary>
    [Label("STORY-9")]
    [FeatureDescription(
@"In order to see all payment details
As a customer
I want to browse historical invoices

Example usage of fluent scenarios")]
    public partial class Invoice_history_feature
    {
        [Label("Ticket-14")]
        [Scenario]
        public async Task Browsing_invoices()
        {
            await Runner
                .AddAsyncSteps(
                    _ => Given_invoice("Invoice-1"),
                    _ => Given_invoice("Invoice-2"))
                .AddSteps(
                    When_I_request_all_historical_invoices)
                .AddSteps(
                    _ => Then_I_should_see_invoices("Invoice-1", "Invoice-2"))
                .RunAsync();
        }
    }
}