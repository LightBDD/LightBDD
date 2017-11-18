using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.NUnit3;

namespace Example.LightBDD.NUnit3.Features
{
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
                .NewScenario()
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