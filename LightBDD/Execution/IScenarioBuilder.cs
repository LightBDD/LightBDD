using System;
using System.Linq.Expressions;

namespace LightBDD.Execution
{
    /// <summary>
    /// Scenario builder interface that allows to build and execute scenario in fluent way.
    /// </summary>
    public interface IScenarioBuilder
    {
        /// <summary>
        /// Specifies that scenario would be executed in dedicated context of <c>TContext</c> type.<br/>
        /// Context instance would be created by calling default constructor.
        /// </summary>
        /// <typeparam name="TContext">Context type</typeparam>
        /// <returns>Scenario builder</returns>
        IScenarioBuilder<TContext> WithContext<TContext>() where TContext : new();
        /// <summary>
        /// Specifies that scenario would be executed in dedicated <c>context</c> instance of <c>TContext</c> type.
        /// </summary>
        /// <typeparam name="TContext">Context type</typeparam>
        /// <param name="instance">Context instance</param>
        /// <returns>Scenario builder</returns>
        IScenarioBuilder<TContext> WithContext<TContext>(TContext instance);

        /// <summary>
        /// Completes scenario build process and runs given steps in order.<br/>
        /// If any step throws, other are not executed and exception is propagated to calling method.
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.NewScenario()
        ///         .Run(
        ///             Given_user_is_about_to_login,
        ///             Given_user_entered_valid_login,
        ///             Given_user_entered_valid_password,
        ///             When_user_clicked_login_button,
        ///             Then_login_is_successful,
        ///             Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_user_is_about_to_login() { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        void Run(params Action[] steps);
        /// <summary>
        /// Completes scenario build process and runs given steps in order.<br/>
        /// If any step throws, other are not executed and exception is propagated to calling method.<br/>
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
        ///     Runner.NewScenario()
        ///         .Run(
        ///             given => Product_is_available_in_products_storage("wooden desk"),
        ///             and => Product_is_available_in_products_storage("wooden shelf"),
        ///             when => Customer_buys_product("wooden desk"),
        ///             and => Customer_buys_product("wooden shelf"),
        ///             then => Invoice_is_sent_to_customer(),
        ///             and => Invoice_contains_product_with_price_of_AMOUNT_pounds("wooden desk", 62),
        ///             and => Invoice_contains_product_with_price_of_AMOUNT_pounds("wooden shelf", 37));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Product_is_available_in_products_storage(string product) { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        void Run(params Expression<Action<StepType>>[] steps);
    }

    /// <summary>
    /// Scenario builder interface that allows to build and execute scenario in fluent way.
    /// This version of builder guarantees that scenario would be executed in dedicated context of <c>TContext</c> type.
    /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
    /// </summary>
    public interface IScenarioBuilder<TContext>
    {
        /// <summary>
        /// Completes scenario build process and executes given steps in order, where all steps share context of <c>TContext</c> type instantiated with default constructor.<br/>
        /// If any step throws, other are not executed and exception is propagated to calling method.<br/>
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     Runner.NewScenario()
        ///         .WithContext&lt;LoginContext&gt;()
        ///         .Run(
        ///             Given_user_is_about_to_login,
        ///             Given_user_entered_valid_login,
        ///             Given_user_entered_valid_password,
        ///             When_user_clicked_login_button,
        ///             Then_login_is_successful,
        ///             Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_user_is_about_to_login(LoginContext context) { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        void Run(params Action<TContext>[] steps);
        /// <summary>
        /// Completes scenario build process and executes given steps in order, where all steps share context of <c>TContext</c> type instantiated with default constructor.<br/>
        /// If any step throws, other are not executed and exception is propagated to calling method.<br/>
        /// Step name is determined on lambda parameter reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// It is suggested that step methods belongs to <c>TContext</c> type, however it is not enforced.<br/>
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
        ///     Runner.NewScenario()
        ///         .WithContext&lt;SpeditionContext&gt;()
        ///         .Run(
        ///             (given, ctx) => ctx.There_is_an_active_customer_with_id("ABC-123"),
        ///             (given, ctx) => ctx.Customer_has_product_in_basket("wooden shelf"),
        ///             (given, ctx) => ctx.Customer_has_product_in_basket("wooden desk"),
        ///             (when, ctx) => ctx.Customer_payment_has_been_finalized(),
        ///             (then, ctx) => ctx.Product_has_been_dispatched_to_customer("wooden shelf"),
        ///             (then, ctx) => ctx.Product_has_been_dispatched_to_customer("wooden desk"));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Customer_has_product_in_basket(string product) { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        void Run(params Expression<Action<StepType, TContext>>[] steps);
    }
}