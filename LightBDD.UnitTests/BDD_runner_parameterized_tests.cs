using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    public class BDD_runner_parameterized_tests : SomeSteps
    {
        private AbstractBDDRunner _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new TestableBDDRunner(GetType(), MockRepository.GenerateMock<IProgressNotifier>());
        }

        #endregion

        [Test]
        [Label("LABEL-57")]
        public void Should_collect_scenario_result_for_parameterized_steps()
        {
            _subject.RunScenario(
                given => Product_is_available_in_product_storage("wooden desk"),
                when => Customer_orders_this_product(),
                then => Customer_receives_invoice_for_product_in_amount_pounds("wooden desk", 62),
                then => Product_is_sent_to_customer());

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for parameterized steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Label, Is.EqualTo("LABEL-57"));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "GIVEN Product \"wooden desk\" is available in product storage", ResultStatus.Passed),
                new StepResultExpectation(2, "WHEN Customer orders this product", ResultStatus.Passed),
                new StepResultExpectation(3, "THEN Customer receives invoice for product \"wooden desk\" in amount \"62\" pounds", ResultStatus.Passed),
                new StepResultExpectation(4, "THEN Product is sent to customer", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_failed_parameterized_steps()
        {
            try
            {
                _subject.RunScenario(
                    given => Customer_has_bought_product("wooden desk"),
                    and => Product_has_usage_marks("wooden desk"),
                    when => Customer_gives_it_back(),
                    then => Product_is_not_accepted("wooden desk"));
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for failed parameterized steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Failed));
            Assert.That(result.StatusDetails, Is.EqualTo("Step 2: Product usage verification is not implemented yet"));
            Assert.That(result.Label, Is.Null);
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "GIVEN Customer has bought product \"wooden desk\"", ResultStatus.Passed),
                new StepResultExpectation(2, "AND Product \"wooden desk\" has usage marks", ResultStatus.Failed,"Product usage verification is not implemented yet"),
                new StepResultExpectation(3, "WHEN Customer gives it back", ResultStatus.NotRun),
                new StepResultExpectation(4, "THEN Product \"wooden desk\" is not accepted", ResultStatus.NotRun)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_parameterized_steps_with_bypassed_steps()
        {
            _subject.RunScenario(
                call => Step_one(),
                call => Step_with_bypass(),
                call => Step_two());

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for parameterized steps with bypassed steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Bypassed));
            Assert.That(result.StatusDetails, Is.EqualTo("Step 2: " + BypassReason));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "CALL Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "CALL Step with bypass", ResultStatus.Bypassed, BypassReason),
                new StepResultExpectation(3, "CALL Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_failed_parameterized_steps_with_bypassed_steps()
        {
            try
            {
                _subject.RunScenario(
                    call => Step_one(),
                    call => Step_with_bypass(),
                    call => Step_throwing_exception());
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for failed parameterized steps with bypassed steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Failed));
            Assert.That(result.StatusDetails, Is.EqualTo("Step 2: " + BypassReason + Environment.NewLine + "Step 3: " + ExceptionText));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "CALL Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "CALL Step with bypass", ResultStatus.Bypassed, BypassReason),
                new StepResultExpectation(3, "CALL Step throwing exception", ResultStatus.Failed, ExceptionText)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_ignored_parameterized_steps_with_bypassed_steps()
        {
            try
            {
                _subject.RunScenario(
                    call => Step_one(),
                    call => Step_with_bypass(),
                    call => Step_with_ignore_assertion());
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for ignored parameterized steps with bypassed steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Ignored));
            Assert.That(result.StatusDetails, Is.EqualTo("Step 2: " + BypassReason + Environment.NewLine + "Step 3: " + IgnoreReason));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "CALL Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "CALL Step with bypass", ResultStatus.Bypassed, BypassReason),
                new StepResultExpectation(3, "CALL Step with ignore assertion", ResultStatus.Ignored, IgnoreReason)
            });
        }

        [Test]
        public void Should_allow_to_reuse_existing_steps_with_action_type()
        {
            _subject.RunScenario(
                _ => Given_product_is_available_in_product_storage("wooden desk"),
                _ => When_customer_orders_this_product(),
                _ => Then_customer_receives_invoice_for_product_in_amount_pounds("wooden desk", 62),
                _ => Then_product_is_sent_to_customer());

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Given product \"wooden desk\" is available in product storage", ResultStatus.Passed),
                new StepResultExpectation(2, "When customer orders this product", ResultStatus.Passed),
                new StepResultExpectation(3, "Then customer receives invoice for product \"wooden desk\" in amount \"62\" pounds", ResultStatus.Passed),
                new StepResultExpectation(4, "Then product is sent to customer", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_with_steps_parameters_inserted_in_proper_places()
        {
            _subject.RunScenario(
                replace => Method_with_parameters_where_param_has_PARAM_value("abc"),
                insert => Method_with_parameters_where_param_is_first_param_on_list("abc_123"),
                others => Method_with_parameters_where_param_is_VALUE("abc", 5, "other param"));

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "REPLACE Method with parameters where param has \"abc\" value", ResultStatus.Passed),
                new StepResultExpectation(2, "INSERT Method with parameters where param \"abc_123\" is first param on list", ResultStatus.Passed),
                new StepResultExpectation(3, "OTHERS Method with parameters where param \"abc\" is \"5\" [lastParam: \"other param\"]", ResultStatus.Passed)
            });
        }

        [Test]
        [TestCase("abc")]
        [TestCase("prod-A")]
        public void Should_capture_method_parameter(string product)
        {
            _subject.RunScenario(call => Product_is_available_in_product_storage(product));

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, string.Format("CALL Product \"{0}\" is available in product storage",product), ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_capture_local_variable()
        {
            string name = GetType().Name;
            _subject.RunScenario(call => Product_is_available_in_product_storage(name));

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, string.Format("CALL Product \"{0}\" is available in product storage",name), ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_capture_method_call()
        {
            _subject.RunScenario(call => Product_is_available_in_product_storage(GetType().ToString()));

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, string.Format("CALL Product \"{0}\" is available in product storage",GetType()), ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_not_allow_steps_with_ref_parameters()
        {
            int x = 5;
            var ex = Assert.Throws<ArgumentException>(() => _subject.RunScenario(when => Set_value(ref x, 3)));
            Assert.That(ex.Message, Is.StringStarting("Steps accepting ref or out parameters are not supported:"));
        }

        [Test]
        public void Should_not_allow_steps_with_out_parameters()
        {
            int x;
            var ex = Assert.Throws<ArgumentException>(() => _subject.RunScenario(when => int.TryParse("abc", out x)));
            Assert.That(ex.Message, Is.StringStarting("Steps accepting ref or out parameters are not supported:"));
        }

        [Test]
        public void Should_capture_parameters_just_before_call_and_evaluate_them_once()
        {
            int x = 0;
            _subject.RunScenario(
                then => Values_equal(1, Add(ref x)),
                then => Values_equal(2, Add(ref x)),
                then => Values_equal(3, Add(ref x)));

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "THEN Values equal [expected: \"1\"] [actual: \"1\"]", ResultStatus.Passed),
                new StepResultExpectation(2, "THEN Values equal [expected: \"2\"] [actual: \"2\"]", ResultStatus.Passed),
                new StepResultExpectation(3, "THEN Values equal [expected: \"3\"] [actual: \"3\"]", ResultStatus.Passed)
            });
        }

        [Test]
        [Label("LABEL-73")]
        public void Should_collect_scenario_result_for_parameterized_steps_executed_on_context()
        {
            _subject.RunScenario<GWTContext>(
                (given, ctx) => ctx.Product_is_available_in_product_storage("wooden desk"),
                (when, ctx) => ctx.Customer_orders_this_product(),
                (then, ctx) => ctx.Customer_receives_invoice_for_product_in_amount_pounds("wooden desk", 62),
                (then, ctx) => ctx.Product_is_sent_to_customer());

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for parameterized steps executed on context"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            Assert.That(result.Label, Is.EqualTo("LABEL-73"));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "GIVEN Product \"wooden desk\" is available in product storage", ResultStatus.Passed),
                new StepResultExpectation(2, "WHEN Customer orders this product", ResultStatus.Passed),
                new StepResultExpectation(3, "THEN Customer receives invoice for product \"wooden desk\" in amount \"62\" pounds", ResultStatus.Passed),
                new StepResultExpectation(4, "THEN Product is sent to customer", ResultStatus.Passed)
            });
        }

        [Test]
        [TestCase("desk")]
        public void Should_print_not_evaluated_parameters_if_step_was_not_executed_yet(string product)
        {
            try
            {
                _subject.RunScenario(
                    given => Customer_has_bought_product(product),
                    when => Customer_gives_it_back(),
                    then => Product_is_put_back_to_product_storage(product),
                    and => Customer_gets_money_back());
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, string.Format("GIVEN Customer has bought product \"{0}\"",product), ResultStatus.Passed),
                new StepResultExpectation(2, "WHEN Customer gives it back", ResultStatus.Ignored,"Not implemented yet"),
                new StepResultExpectation(3, "THEN Product \"<?>\" is put back to product storage", ResultStatus.NotRun),
                new StepResultExpectation(4, "AND Customer gets money back", ResultStatus.NotRun)
            });
        }

        [Test]
        public void Should_capture_constant_parameters_even_if_step_was_not_executed_yet()
        {
            try
            {
                _subject.RunScenario(
                    given => Customer_has_bought_product("desk"),
                    when => Customer_gives_it_back(),
                    then => Product_is_put_back_to_product_storage("desk"),
                    and => Customer_gets_money_back());
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "GIVEN Customer has bought product \"desk\"", ResultStatus.Passed),
                new StepResultExpectation(2, "WHEN Customer gives it back", ResultStatus.Ignored,"Not implemented yet"),
                new StepResultExpectation(3, "THEN Product \"desk\" is put back to product storage", ResultStatus.NotRun),
                new StepResultExpectation(4, "AND Customer gets money back", ResultStatus.NotRun)
            });
        }

        [Test]
        public void Should_capture_constant_parameters_of_different_type_even_if_step_was_not_executed_yet()
        {
            try
            {
                _subject.RunScenario(
                    call => Ignored_method(),
                    call => Method_with_parameter("abc"),
                    call => Method_with_parameter(1),
                    call => Method_with_parameter(3.5),
                    call => Method_with_parameter(null),
                    call => Method_with_parameter(22.67m),
                    call => Method_with_parameter('a'),
                    call => Method_with_parameter(typeof(object)),
                    call => Method_with_parameter_and_OTHER("abc", new DateTime(2014, 05, 31)));
            }
            catch { }

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "CALL Ignored method", ResultStatus.Ignored,"ignored"),
                new StepResultExpectation(2, "CALL Method with parameter \"abc\"", ResultStatus.NotRun),
                new StepResultExpectation(3, "CALL Method with parameter \"1\"", ResultStatus.NotRun),
                new StepResultExpectation(4, "CALL Method with parameter \"3.5\"", ResultStatus.NotRun),
                new StepResultExpectation(5, "CALL Method with parameter \"\"", ResultStatus.NotRun),
                new StepResultExpectation(6, "CALL Method with parameter \"22.67\"", ResultStatus.NotRun),
                new StepResultExpectation(7, "CALL Method with parameter \"a\"", ResultStatus.NotRun),
                new StepResultExpectation(8, "CALL Method with parameter \"System.Object\"", ResultStatus.NotRun),
                new StepResultExpectation(9, "CALL Method with parameter \"abc\" and \"<?>\"", ResultStatus.NotRun)
            });
        }

        [Test]
        public void Should_capture_parameters_formatted_with_invariant_culture()
        {
            _subject.RunScenario(
                call => Method_with_parameter(1234234),
                call => Method_with_parameter(3.53),
                call => Method_with_parameter(22.67m),
                call => Method_with_parameter(new DateTime(2014, 05, 31, 17, 31, 27)));

            var result = _subject.Result.Scenarios.Single();
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "CALL Method with parameter \"1234234\"", ResultStatus.Passed),
                new StepResultExpectation(2, "CALL Method with parameter \"3.53\"", ResultStatus.Passed),
                new StepResultExpectation(3, "CALL Method with parameter \"22.67\"", ResultStatus.Passed),
                new StepResultExpectation(4, "CALL Method with parameter \"05/31/2014 17:31:27\"", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_capture_step_name_details()
        {
            try
            {
                _subject.RunScenario(
                    call => Method_with_parameter(42),
                    call => Step_throwing_exception_MESSAGE("abc"),
                    call => Method_with_parameter(34),
                    call => Method_with_parameter(TimeSpan.FromSeconds(10)));
            }
            catch (InvalidOperationException) { }
            var steps = _subject.Result.Scenarios.Single().Steps.ToArray();
            AssertStepName(steps[0], "CALL", "Method with parameter \"{0}\"", new StepParameterExpectation("42", true));
            AssertStepName(steps[1], "CALL", "Step throwing exception \"{0}\"", new StepParameterExpectation("abc", true));
            AssertStepName(steps[2], "CALL", "Method with parameter \"{0}\"", new StepParameterExpectation("34", true));
            AssertStepName(steps[3], "CALL", "Method with parameter \"{0}\"", new StepParameterExpectation("<?>", false));
        }

        [Test]
        public void Should_capture_step_name_with_parameters_having_custom_formatters()
        {
            _subject.RunScenario(call => Method_with_amount_percentage_and_other_value(42, 42, 42));

            var steps = _subject.Result.Scenarios.Single().Steps.ToArray();
            StepResultExpectation.Assert(steps, new[]
            {
                new StepResultExpectation(1, "CALL Method with amount \"$42.00\" percentage \"42%\" and other value \"42\"", ResultStatus.Passed)
            });

            AssertStepName(steps[0], "CALL", "Method with amount \"{0}\" percentage \"{1}\" and other value \"{2}\"",
                new StepParameterExpectation("$42.00", true),
                new StepParameterExpectation("42%", true),
                new StepParameterExpectation("42", true));
        }

        [Test]
        public void Should_not_allow_to_use_multiple_parameter_formatters_at_once()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _subject.RunScenario(call => Step_with_multiple_formatters_on_one_parameter("abc")));
            Assert.That(ex.Message, Is.EqualTo("Parameter can contain only one attribute ParameterFormatterAttribute. Parameter: parameter, Detected attributes: FormatAttribute, OtherFormatter"));
        }

        private static void AssertStepName(IStepResult step, string stepTypeName, string nameFormat, params StepParameterExpectation[] expectedParameters)
        {
            Assert.That(step.StepName, Is.Not.Null);
            Assert.That(step.StepName.NameFormat, Is.EqualTo(nameFormat));
            Assert.That(step.StepName.StepTypeName, Is.EqualTo(stepTypeName));
            AssertStepParameters(step.StepName.Parameters, expectedParameters);
        }

        private static void AssertStepParameters(IEnumerable<IStepParameter> parameters, IEnumerable<StepParameterExpectation> expectedParameters)
        {
            var actual = parameters.Select(p => string.Format("Value={0}, Evaluated={1}", p.FormattedValue, p.IsEvaluated)).ToArray();
            var expected = expectedParameters.Select(p => string.Format("Value={0}, Evaluated={1}", p.FormattedValue, p.IsEvaluated)).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }

        private void Method_with_parameter_and_OTHER(object parameter, object other) { }
        private void Method_with_parameter(double parameter) { }
        private void Method_with_parameter(decimal parameter) { }
        private void Method_with_parameter(char parameter) { }
        private void Method_with_parameter(int parameter) { }
        private void Method_with_parameter(object parameter) { }

        private void Ignored_method()
        {
            Assert.Ignore("ignored");
        }

        private void Customer_gets_money_back()
        {
        }

        private void Product_is_put_back_to_product_storage(string product)
        {
        }

        private void Product_has_usage_marks(string product)
        {
            throw new NotImplementedException("Product usage verification is not implemented yet");
        }

        private void Product_is_not_accepted(string product)
        {
        }

        private void Customer_gives_it_back()
        {
            Assert.Ignore("Not implemented yet");
        }

        private void Customer_has_bought_product(string product)
        {
        }

        private int Add(ref int x)
        {
            return ++x;
        }

        private void Values_equal(int expected, int actual)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        private void Set_value(ref int i, int val)
        {
            i = val;
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

        private void Given_product_is_available_in_product_storage(string product)
        {
        }

        private void When_customer_orders_this_product()
        {
        }

        private void Then_customer_receives_invoice_for_product_in_amount_pounds(string product, int amount)
        {
        }

        private void Then_product_is_sent_to_customer()
        {
        }

        private void Method_with_amount_percentage_and_other_value(
            [Format("{0:$0.00}")] decimal amount,
            [Format("{0:0\\%}")] decimal percentage,
            decimal value)
        {
        }

        private void Step_with_multiple_formatters_on_one_parameter([Format("{0}"), OtherFormatter]string parameter)
        {
        }
    }
    class OtherFormatter : ParameterFormatterAttribute
    {
        public override string Format(object parameter)
        {
            return "abc";
        }
    }
    class GWTContext
    {
        public void Product_is_available_in_product_storage(string product)
        {

        }

        public void Customer_orders_this_product()
        {

        }

        public void Customer_receives_invoice_for_product_in_amount_pounds(string product, int amount)
        {
        }

        public void Product_is_sent_to_customer()
        {
        }
    }
}