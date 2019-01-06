using LightBDD.Core.Execution;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Extended
{
    /// <summary>
    /// Extensions class allowing to use extended scenario syntax for running LightBDD tests.
    /// </summary>
    [DebuggerStepThrough]
    public static class ExtendedScenarioExtensions
    {
        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined from the method name in which <see cref="RunScenario{TContext}"/>() method was called.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on method in which <see cref="RunScenario{TContext}"/>() method was called.<br/>
        /// Step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is executed with context, the context instance is provided with lambda parameter.
        /// Please note that rules for placing parameter values in step name are as follows, where first matching rule would be used:
        /// <list type="bullet">
        /// <item><description>it will replace first occurrence of variable name written in capital letters (<c>void Price_is_AMOUNT_dollars(int amount)</c> => <c>Price is "27" dollars</c>)</description></item>
        /// <item><description>it will placed after first occurrence of variable name (<c>void Product_is_in_stock(string product)</c> => <c>Product "desk" is in stock</c>)</description></item>
        /// <item><description>it will placed at the end of step name (<c>void Product_is_in_stock(string productId)</c> => <c>Product is in stock [productId: "ABC123"]</c>)</description></item>
        /// </list>
        /// <para>
        /// Example usage for scenarios with no context:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-2")]
        /// public void Receiving_invoice_for_products()
        /// {
        ///     Runner.RunScenario(
        ///         _ => Given_product_is_available_in_product_storage("wooden desk"),
        ///         _ => When_customer_buys_product("wooden desk"),
        ///         _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_product_is_available_in_product_storage(string product) { /* ... */ }
        /// </code>
        /// </para>
        /// <para>
        /// Example usage for scenarios with context:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-3")]
        /// public void Should_dispatch_product_after_payment_is_finalized()
        /// {
        ///     Runner.WithContext&lt;SpeditionContext&gt;().RunScenario(
        ///         _ => _.Given_there_is_an_active_customer_with_id("ABC-123"),
        ///         _ => _.Given_the_customer_has_product_in_basket("wooden shelf"),
        ///         _ => _.When_the_customer_payment_finalizes(),
        ///         _ => _.Then_product_should_be_dispatched_to_the_customer("wooden shelf"));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// class SpeditionContext
        /// {
        ///     void Given_product_is_available_in_product_storage(string product) { /* ... */ }
        /// }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static void RunScenario<TContext>(this IBddRunner<TContext> runner, params Expression<Action<TContext>>[] steps)
        {
            try
            {
                runner
                    .AddSteps(steps)
                    .Integrate().Core
                    .WithCapturedScenarioDetails()
                    .Build()
                    .ExecuteSync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined from the method name in which <see cref="RunScenarioAsync{TContext}"/>() method was called.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on method in which <see cref="RunScenarioAsync{TContext}"/>() method was called.<br/>
        /// Step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is executed with context, the context instance is provided with lambda parameter.
        /// Please note that rules for placing parameter values in step name are as follows, where first matching rule would be used:
        /// <list type="bullet">
        /// <item><description>it will replace first occurrence of variable name written in capital letters (<c>Task Price_is_AMOUNT_dollars(int amount)</c> => <c>Price is "27" dollars</c>)</description></item>
        /// <item><description>it will placed after first occurrence of variable name (<c>Task Product_is_in_stock(string product)</c> => <c>Product "desk" is in stock</c>)</description></item>
        /// <item><description>it will placed at the end of step name (<c>Task Product_is_in_stock(string productId)</c> => <c>Product is in stock [productId: "ABC123"]</c>)</description></item>
        /// </list>
        /// <para>
        /// Example usage for scenarios with no context:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-2")]
        /// public Task Receiving_invoice_for_products()
        /// {
        ///     return Runner.RunScenarioAsync(
        ///         _ => Given_product_is_available_in_product_storage("wooden desk"),
        ///         _ => When_customer_buys_product("wooden desk"),
        ///         _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// async Task Given_product_is_available_in_product_storage(string product) { /* ... */ }
        /// </code>
        /// </para>
        /// <para>
        /// Example usage for scenarios with context:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-3")]
        /// public Task Should_dispatch_product_after_payment_is_finalized()
        /// {
        ///     return Runner.WithContext&lt;SpeditionContext&gt;().RunScenarioAsync(
        ///         _ => _.Given_there_is_an_active_customer_with_id("ABC-123"),
        ///         _ => _.Given_the_customer_has_product_in_basket("wooden shelf"),
        ///         _ => _.When_the_customer_payment_finalizes(),
        ///         _ => _.Then_product_should_be_dispatched_to_the_customer("wooden shelf"));
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// class SpeditionContext
        /// {
        ///     async Task Given_product_is_available_in_product_storage(string product) { /* ... */ }
        /// }
        /// </code>
        /// </para>
        /// </summary>
        /// <remarks>This is an asynchronous method and should be awaited.</remarks>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static async Task RunScenarioAsync<TContext>(this IBddRunner<TContext> runner, params Expression<Func<TContext, Task>>[] steps)
        {
            try
            {
                await runner
                    .AddAsyncSteps(steps)
                    .Integrate().Core
                    .WithCapturedScenarioDetails()
                    .Build()
                    .ExecuteAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
    }
}
