using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    public class BDD_runner_parameterized_tests
    {
        private AbstractBDDRunner _subject;
        private IProgressNotifier _progressNotifier;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _subject = new TestableBDDRunner(GetType(), _progressNotifier);
        }

        #endregion

        [Test]
        [Label("LABEL-57")]
        public void Should_collect_scenario_result_for_parameterized_steps()
        {
            _subject.RunFormalizedScenario(
                given => Product_is_available_in_product_storage("wooden desk"),
                when => Customer_orders_this_product(),
                then => Customer_receives_invoice_for_product_in_amount_pounds("wooden desk", 62),
                then => Product_is_sent_to_customer());

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for parameterized steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Label, Is.EqualTo("LABEL-57"));
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "GIVEN Product \"wooden desk\" is available in product storage", ResultStatus.Passed),
                new StepResult(2, "WHEN Customer orders this product", ResultStatus.Passed),
                new StepResult(3, "THEN Customer receives invoice for product \"wooden desk\" in amount \"62\" pounds", ResultStatus.Passed),
                new StepResult(4, "THEN Product is sent to customer", ResultStatus.Passed)
            }));
        }

        [Test]
        public void Should_collect_scenario_result_with_steps_parameters_inserted_in_proper_places()
        {
            _subject.RunFormalizedScenario(
                replace => Method_with_parameters_where_param_has_PARAM_value("abc"),
                insert => Method_with_parameters_where_param_is_first_param_on_list("abc_123"),
                others => Method_with_parameters_where_param_is_VALUE("abc", 5, "other param"));

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Steps, Is.EqualTo(new[]
            {
                new StepResult(1, "REPLACE Method with parameters where param has \"abc\" value", ResultStatus.Passed),
                new StepResult(2, "INSERT Method with parameters where param \"abc_123\" is first param on list", ResultStatus.Passed),
                new StepResult(3, "OTHERS Method with parameters where param \"abc\" is \"5\" [lastParam: \"other param\"]", ResultStatus.Passed)
            }));
        }

        private void Method_with_parameters_where_param_is_VALUE(string param, int value, string lastParam)
        {
        }

        private void Method_with_parameters_where_param_is_first_param_on_list(string param)
        {
        }

        private void Method_with_parameters_where_param_has_PARAM_value(string param)
        {
        }

        private void Product_is_available_in_product_storage(string product)
        {
        }

        private void Customer_orders_this_product()
        {
        }

        private void Customer_receives_invoice_for_product_in_amount_pounds(string product, int amount)
        {
        }

        private void Product_is_sent_to_customer()
        {
        }
    }
}