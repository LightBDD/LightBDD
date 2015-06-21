using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using LightBDD.Execution;
using LightBDD.Execution.Implementation;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD
{
    /// <summary>
    /// Abstract class for executing behavior test scenarios.
    /// </summary>
    [DebuggerStepThrough]
    public abstract class AbstractBDDRunner
    {
        private readonly FeatureResult _result;
        private readonly TestMetadataProvider _metadataProvider;
        private readonly IScenarioExecutor _executor;
        private readonly IStepsConverter _stepsConverter;

        /// <summary>
        /// Progress notifier.
        /// </summary>
        public IProgressNotifier ProgressNotifier { get; private set; }

        /// <summary>
        /// Returns feature execution result.
        /// </summary>
        public IFeatureResult Result
        {
            get { return _result; }
        }

        /// <summary>
        /// Initializes runner for given feature test class type with ConsoleProgressNotifier.
        /// Given featureTestClass type Name is used as feature name.
        /// If test class is annotated with [FeatureDescription] attribute or implementation specific description attribute, it's content is used as feature description.
        /// </summary>
        /// <param name="featureTestClass">Test class type.</param>
        /// <param name="metadataProvider">Test metadata provider.</param>
        protected AbstractBDDRunner(Type featureTestClass, TestMetadataProvider metadataProvider)
            : this(featureTestClass, metadataProvider, new ConsoleProgressNotifier())
        {
        }

        /// <summary>
        /// Initializes runner for given feature test class type with given progress notifier.
        /// Given featureTestClass type Name is used as feature name.
        /// If test class is annotated with [FeatureDescription] attribute or implementation specific description attribute, it's content is used as feature description.
        /// </summary>
        /// <param name="featureTestClass">Test class type.</param>
        /// <param name="progressNotifier">Progress notifier.</param>
        /// <param name="metadataProvider">Test metadata provider.</param>
        protected AbstractBDDRunner(Type featureTestClass, TestMetadataProvider metadataProvider, IProgressNotifier progressNotifier)
        {
            if (featureTestClass == null)
                throw new ArgumentNullException("featureTestClass");
            if (metadataProvider == null)
                throw new ArgumentNullException("metadataProvider");
            if (progressNotifier == null)
                throw new ArgumentNullException("progressNotifier");

            _metadataProvider = metadataProvider;
            ProgressNotifier = progressNotifier;
            _stepsConverter = new StepsConverter(_metadataProvider, MapExceptionToStatus);
            _result = new FeatureResult(
                _metadataProvider.GetFeatureName(featureTestClass),
                _metadataProvider.GetFeatureDescription(featureTestClass),
                _metadataProvider.GetFeatureLabel(featureTestClass));
            _executor = new ScenarioExecutor(ProgressNotifier);
            _executor.ScenarioExecuted += _result.AddScenario;
            ProgressNotifier.NotifyFeatureStart(_result.Name, _result.Description, _result.Label);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order, where all steps share context of <c>TContext</c> type instantiated with default constructor.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined on the method name in which <c>RunScenario()</c> method was called.<br/>
        /// Scenario label is determined on <c>[Label]</c> attribute applied on method in which <c>RunScenario()</c> method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has <c>[assembly:Debuggable(true, true)]</c> attribute), or calling method has to have <c>[MethodImpl(MethodImplOptions.NoInlining)]</c> attribute in order to properly determine scenario name.<br/>
        /// Step name is determined on corresponding action name.<br/>
        ///
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario&lt;LoginContext&gt;(
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login(LoginContext context) { /* ... */ }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario<TContext>(params Action<TContext>[] steps) where TContext : new()
        {
            NewScenario(GetScenarioMethod())
                .WithContext<TContext>()
                .Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order, where all steps share context of <c>TContext</c> type instantiated with default constructor.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is specified in parameter list.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// 
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario&lt;LoginContext&gt;("My successful login",
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login(LoginContext context) { /* ... */ }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithContext<TContext>().Run(steps) instead.")]
        public void RunScenario<TContext>(string scenarioName, params Action<TContext>[] steps) where TContext : new()
        {
            NewScenario(scenarioName)
                .WithContext<TContext>()
                .Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order, where all steps share context of <c>TContext</c> type instantiated with default constructor.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name and label are specified in parameter list.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario&lt;LoginContext&gt;("My successful login", "Ticket-1",
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login(LoginContext context) { /* ... */ }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Label associated with this scenario.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithLabel(label).WithContext<TContext>().Run(steps) instead.")]
        public void RunScenario<TContext>(string scenarioName, string label, params Action<TContext>[] steps) where TContext : new()
        {
            NewScenario(scenarioName)
                .WithLabel(label)
                .WithContext<TContext>()
                .Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order, where all steps share given <c>context</c> instance of <c>TContext</c> type.
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined on the method name in which <c>RunScenario()</c> method was called.<br/>
        /// Scenario label is determined on <c>[Label]</c> attribute applied on method in which <c>RunScenario()</c> method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has <c>[assembly:Debuggable(true, true)]</c> attribute), or calling method has to have <c>[MethodImpl(MethodImplOptions.NoInlining)]</c> attribute in order to properly determine scenario name.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario(new LoginContext(),
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login(LoginContext context) { /* ... */ }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="context">Context instance that would be shared between all steps.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario<TContext>(TContext context, params Action<TContext>[] steps)
        {
            NewScenario(GetScenarioMethod())
                .WithContext(context)
                .Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order, where all steps share given <c>context</c> instance of <c>TContext</c> type.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is specified in parameter list.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario(new LoginContext(), "My successful login",
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login(LoginContext context) { /* ... */ }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="context">Context instance that would be shared between all steps.</param>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithContext(context).Run(steps) instead.")]
        public void RunScenario<TContext>(TContext context, string scenarioName, params Action<TContext>[] steps)
        {
            NewScenario(scenarioName)
                .WithContext(context)
                .Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order, where all steps share given <c>context</c> instance of <c>TContext</c> type.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name and label are specified in parameter list.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario(new LoginContext(), "My successful login", "Ticket-1",
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login(LoginContext context) { /* ... */ }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="context">Context instance that would be shared between all steps.</param>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Label associated with this scenario.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithLabel(label).WithContext(context).Run(steps) instead.")]
        public void RunScenario<TContext>(TContext context, string scenarioName, string label, params Action<TContext>[] steps)
        {
            NewScenario(scenarioName)
                .WithLabel(label)
                .WithContext(context)
                .Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined on the method name in which <c>RunScenario()</c> method was called.<br/>
        /// Scenario label is determined on <c>[Label]</c> attribute applied on method in which <c>RunScenario()</c> method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has <c>[assembly:Debuggable(true, true)]</c> attribute), or calling method has to have <c>[MethodImpl(MethodImplOptions.NoInlining)]</c> attribute in order to properly determine scenario name.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario(
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login() { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario(params Action[] steps)
        {
            NewScenario(GetScenarioMethod()).Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined on the method name in which <c>RunScenario()</c> method was called.<br/>
        /// Scenario label is determined on <c>[Label]</c> attribute applied on method in which <c>RunScenario()</c> method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has <c>[assembly:Debuggable(true, true)]</c> attribute), or calling method has to have <c>[MethodImpl(MethodImplOptions.NoInlining)]</c> attribute in order to properly determine scenario name.<br/>
        /// Step name is determined on lambda parameter reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// Please note that rules for placing parameter values in step name are as follows, where first matching rule would be used:
        /// <list type="bullet">
        /// <item><description>it will replace first occurrence of variable name written in capital letters (<c>void Price_is_AMOUNT_dollars(int amount)</c> => <c>Price is "27" dollars</c>)</description></item>
        /// <item><description>it will placed after first occurrence of variable name (<c>void Product_is_in_stock(string product)</c> => <c>Product "desk" is in stock</c>)</description></item>
        /// <item><description>it will placed at the end of step name (<c>void Product_is_in_stock(string productId)</c> => <c>Product is in stock [productId: "ABC123"]</c>)</description></item>
        /// </list>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Receiving_invoice_for_products()
        /// {
        ///     Runner.RunScenario(
        ///         given => Product_is_available_in_product_storage("wooden desk"),
        ///         and => Product_is_available_in_product_storage("wooden shelf"),
        ///         when => Customer_buys_product("wooden desk"),
        ///         and => Customer_buys_product("wooden shelf"),
        ///         then => An_invoice_should_be_sent_to_the_customer(),
        ///         and => Invoice_contains_product_with_price_of_AMOUNT_pounds("wooden desk", 62),
        ///         and => Invoice_contains_product_with_price_of_AMOUNT_pounds("wooden shelf", 37));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Product_is_available_in_product_storage(string product) { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario(params Expression<Action<StepType>>[] steps)
        {
            NewScenario(GetScenarioMethod()).Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order, where all steps share context of <c>TContext</c> type instantiated with default constructor.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined on the method name in which <c>RunScenario()</c> method was called.<br/>
        /// Scenario label is determined on <c>[Label]</c> attribute applied on method in which <c>RunScenario()</c> method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has <c>[assembly:Debuggable(true, true)]</c> attribute), or calling method has to have <c>[MethodImpl(MethodImplOptions.NoInlining)]</c> attribute in order to properly determine scenario name.<br/>
        /// Step name is determined on lambda parameter reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// It is suggested that step methods belongs to <c>TContext</c> type, however it is not required.<br/>
        /// Please note that rules for placing parameter values in step name are as follows, where first matching rule would be used:
        /// <list type="bullet">
        /// <item><description>it will replace first occurrence of variable name written in capital letters (<c>void Price_is_AMOUNT_dollars(int amount)</c> => <c>Price is "27" dollars</c>)</description></item>
        /// <item><description>it will placed after first occurrence of variable name (<c>void Product_is_in_stock(string product)</c> => <c>Product "desk" is in stock</c>)</description></item>
        /// <item><description>it will placed at the end of step name (<c>void Product_is_in_stock(string productId)</c> => <c>Product is in stock [productId: "ABC123"]</c>)</description></item>
        /// </list>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-5")]
        /// public void Should_dispatch_product_after_payment_is_finalized()
        /// {
        ///     Runner.RunScenario&lt;SpeditionContext&gt;(
        ///         (given, ctx) => ctx.There_is_an_active_customer_with_id("ABC-123"),
        ///         (and, ctx) => ctx.The_customer_has_product_in_basket("wooden shelf"),
        ///         (and, ctx) => ctx.The_customer_has_product_in_basket("wooden desk"),
        ///         (when, ctx) => ctx.The_customer_payment_finalizes(),
        ///         (then, ctx) => ctx.Product_should_be_dispatched_to_the_customer("wooden shelf"),
        ///         (and, ctx) => ctx.Product_should_be_dispatched_to_the_customer("wooden desk"));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void The_customer_has_product_in_basket(string product) { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario<TContext>(params Expression<Action<StepType, TContext>>[] steps) where TContext : new()
        {
            NewScenario(GetScenarioMethod())
                .WithContext<TContext>()
                .Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is specified in parameter list.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario("My successful login",
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login() { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).Run(steps) instead.")]
        public void RunScenario(string scenarioName, params Action[] steps)
        {
            NewScenario(scenarioName).Run(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name and label are specified in parameter list.<br/>
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario("My successful login", "Ticket-1",
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login() { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Label associated with this scenario.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithLabel(label).Run(steps) instead.")]
        public void RunScenario(string scenarioName, string label, params Action[] steps)
        {
            NewScenario(scenarioName)
                .WithLabel(label)
                .Run(steps);
        }

        /// <summary>
        /// Starts new scenario build process, where scenario name is specified by <c>scenarioName</c> parameter.<br/>
        /// Method returns scenario builder object allowing to specify optional label and execution of scenario steps.<br/>
        /// Build process is finished with calling one of <c>Run()</c> method family.<br/>
        /// Please note that scenario would not be added to result list, until <c>Run()</c> method is called.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.NewScenario("My successful login")
        ///         .WithLabel("Ticket-1")
        ///         .Run(
        ///             Given_the_user_is_about_to_login,
        ///             Given_the_user_entered_valid_login,
        ///             Given_the_user_entered_valid_password,
        ///             When_the_user_clicks_login_button,
        ///             Then_the_login_operation_should_be_successful,
        ///             Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// </summary>
        /// <param name="scenarioName">Name of scenario.</param>
        /// <returns>Scenario builder.</returns>
        public ICustomizedScenarioBuilder NewScenario(string scenarioName)
        {
            return new ScenarioBuilder(_stepsConverter, _executor, scenarioName);
        }

        /// <summary>
        /// Starts new scenario build process, where scenario name is determined on the method name in which <c>NewScenario()</c> method was called.<br/>
        /// Scenario label is determined on <c>[Label]</c> attribute applied on method in which <c>RunScenario()</c> method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has <c>[assembly:Debuggable(true, true)]</c> attribute), or calling method has to have <c>[MethodImpl(MethodImplOptions.NoInlining)]</c> attribute in order to properly determine scenario name.<br/>
        /// 
        /// Scenario build process can be finalized later by calling one of <c>Run()</c> method family.<br/>
        /// Please note that scenario would not be added to result list, until <c>Run()</c> method is called.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     Runner.NewScenario()
        ///         .Run(
        ///             Given_the_user_is_about_to_login,
        ///             Given_the_user_entered_valid_login,
        ///             Given_the_user_entered_valid_password,
        ///             When_the_user_clicks_login_button,
        ///             Then_the_login_operation_should_be_successful,
        ///             Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public IScenarioBuilder NewScenario()
        {
            return NewScenario(GetScenarioMethod());
        }

        private ICustomizedScenarioBuilder NewScenario(MethodBase scenarioMethod)
        {
            return NewScenario(_metadataProvider.GetScenarioName(scenarioMethod))
                .WithLabel(_metadataProvider.GetScenarioLabel(scenarioMethod))
                .WithCategories(_metadataProvider.GetScenarioCategories(scenarioMethod).ToArray());
        }

        /// <summary>
        /// Maps implementation specific exception to ResultStatus.
        /// </summary>
        protected abstract ResultStatus MapExceptionToStatus(Type exceptionType);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static MethodBase GetScenarioMethod()
        {
            return new StackTrace().GetFrame(2).GetMethod();
        }
    }
}
