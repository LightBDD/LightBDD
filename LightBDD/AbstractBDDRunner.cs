using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using LightBDD.Execution;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD
{
    /// <summary>
    /// Abstract class for executing behavior test scenarios.
    /// </summary>
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
            _metadataProvider = metadataProvider;
            ProgressNotifier = progressNotifier;
            _stepsConverter = new StepsConverter(_metadataProvider, MapExceptionToStatus);
            _result = new FeatureResult(_metadataProvider.GetFeatureName(featureTestClass), _metadataProvider.GetFeatureDescription(featureTestClass), _metadataProvider.GetFeatureLabel(featureTestClass));
            _executor = new ScenarioExecutor(ProgressNotifier);
            _executor.ScenarioExecuted += _result.AddScenario;
            ProgressNotifier.NotifyFeatureStart(_result.Name, _result.Description, _result.Label);
        }

        /// <summary>
        /// Runs test scenario with by executing given steps in order, where all steps share context of <c>TContext</c> type instantiated with default constructor.
        /// If given step throws, other are not executed.
        /// Scenario name is determined on method name in which RunScenario() method was called.<br/>
        /// Scenario labels are determined on [Label] attributes applied on method in which RunScenario() method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has [Debuggable(true, true)] attribute), or test method has to have [MethodImpl(MethodImplOptions.NoInlining)] attribute in order to properly determine scenario name.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario&lt;LoginContext&gt;(
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario<TContext>(params Action<TContext>[] steps) where TContext : new()
        {
            NewScenario(GetScenarioMethod()).Execute(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order, where all steps share context of <c>TContext</c> type instantiated with default constructor.
        /// If given step throws, other are not executed.
        /// Scenario name is specified in parameter list.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario&lt;LoginContext&gt;("My successful login",
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).Execute(steps) instead.")]
        public void RunScenario<TContext>(string scenarioName, params Action<TContext>[] steps) where TContext : new()
        {
            NewScenario(scenarioName)
                .Execute(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order, where all steps share context of <c>TContext</c> type instantiated with default constructor.
        /// If given step throws, other are not executed.
        /// Scenario name is specified in parameter list.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario&lt;LoginContext&gt;("My successful login", "Ticket-1",
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Label associated with this scenario.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithLabel(label).Execute(steps) instead.")]
        public void RunScenario<TContext>(string scenarioName, string label, params Action<TContext>[] steps) where TContext : new()
        {
            NewScenario(scenarioName)
                .WithLabel(label)
                .Execute(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order, where all steps share given <c>context</c> instance of <c>TContext</c> type.
        /// If given step throws, other are not executed.
        /// Scenario name is determined on method name in which RunScenario() method was called.<br/>
        /// Scenario labels are determined on [Label] attributes applied on method in which RunScenario() method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has [Debuggable(true, true)] attribute), or test method has to have [MethodImpl(MethodImplOptions.NoInlining)] attribute in order to properly determine scenario name.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario(new LoginContext(),
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="context">Context instance that would be shared between all steps.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario<TContext>(TContext context, params Action<TContext>[] steps)
        {
            NewScenario(GetScenarioMethod()).Execute(context, steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order, where all steps share given <c>context</c> instance of <c>TContext</c> type.
        /// If given step throws, other are not executed.
        /// Scenario name is specified in parameter list.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario(new LoginContext(), "My successful login",
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="context">Context instance that would be shared between all steps.</param>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).Execute(context, steps) instead.")]
        public void RunScenario<TContext>(TContext context, string scenarioName, params Action<TContext>[] steps)
        {
            NewScenario(scenarioName).Execute(context, steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order, where all steps share given <c>context</c> instance of <c>TContext</c> type.
        /// If given step throws, other are not executed.
        /// Scenario name is specified in parameter list.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario(new LoginContext(), "My successful login", "Ticket-1",
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="context">Context instance that would be shared between all steps.</param>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Label associated with this scenario.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithLabel(label).Execute(context, steps) instead.")]
        public void RunScenario<TContext>(TContext context, string scenarioName, string label, params Action<TContext>[] steps)
        {
            NewScenario(scenarioName)
                .WithLabel(label)
                .Execute(context, steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order.
        /// If given step throws, other are not executed.
        /// Scenario name is determined on method name in which RunScenario() method was called.<br/>
        /// Scenario labels are determined on [Label] attributes applied on method in which RunScenario() method was called.<br/>
        /// Please note that test project has to be compiled in DEBUG mode (assembly has [Debuggable(true, true)] attribute), or test method has to have [MethodImpl(MethodImplOptions.NoInlining)] attribute in order to properly determine scenario name.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario(
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunScenario(params Action[] steps)
        {
            NewScenario(GetScenarioMethod()).Execute(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order.
        /// If given step throws, other are not executed.
        /// Scenario name is specified in parameter list.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario("My successful login",
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).Execute(steps) instead.")]
        public void RunScenario(string scenarioName, params Action[] steps)
        {
            NewScenario(scenarioName).Execute(steps);
        }

        /// <summary>
        /// Runs test scenario by executing given steps in order.
        /// If given step throws, other are not executed.
        /// Scenario name is specified in parameter list.
        /// Step name is determined on corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.RunScenario("My successful login", "Ticket-1",
        ///         Given_user_is_about_to_login,
        ///         Given_user_entered_valid_login,
        ///         Given_user_entered_valid_password,
        ///         When_user_clicked_login_button,
        ///         Then_login_is_successful,
        ///         Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Label associated with this scenario.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        [Obsolete("This method is obsolete and would be deleted in next release. Please use NewScenario(scenarioName).WithLabel(label).Execute(steps) instead.")]
        public void RunScenario(string scenarioName, string label, params Action[] steps)
        {
            NewScenario(scenarioName)
                .WithLabel(label)
                .Execute(steps);
        }

        /// <summary>
        /// Starts new scenario build process, where scenario name is specified by scenarioName parameter.
        /// Method returns scenario builder object allowing to specify optional Label and execution of scenario steps.
        /// Build process is finished with Execute() method. Please note that scenario would not be added to result list, until Execute() method is called.
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.NewScenario("My successful login")
        ///         .WithLabel("Ticket-1")
        ///         .Execute(
        ///             Given_user_is_about_to_login,
        ///             Given_user_entered_valid_login,
        ///             Given_user_entered_valid_password,
        ///             When_user_clicked_login_button,
        ///             Then_login_is_successful,
        ///             Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <param name="scenarioName">Name of scenario.</param>
        /// <returns>Scenario builder.</returns>
        public IScenarioBuilder NewScenario(string scenarioName)
        {
            return new ScenarioBuilder(_stepsConverter, _executor, scenarioName);
        }

        private IScenarioBuilder NewScenario(MethodBase scenarioMethod)
        {
            return NewScenario(_metadataProvider.GetScenarioName(scenarioMethod))
                .WithLabel(_metadataProvider.GetScenarioLabel(scenarioMethod));
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
