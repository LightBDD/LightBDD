using LightBDD.Framework.Scenarios.Extended.Implementation;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Extended
{
    /// <summary>
    /// Extensions class allowing to use extended scenario syntax for defining LightBDD scenarios in fluent way.
    /// </summary>
    [DebuggerStepThrough]
    public static class ExtendedScenarioBuilderExtensions
    {
        /// <summary>
        /// Adds steps specified by <paramref name="steps"/> parameter.<br/>
        /// The steps would be executed in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
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
        /// builder.AddSteps(
        ///     _ => Given_product_is_available_in_product_storage("wooden desk"),
        ///     _ => When_customer_buys_product("wooden desk"),
        ///     _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62));
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_product_is_available_in_product_storage(string product) { /* ... */ }
        /// </code>
        /// </para>
        /// <para>
        /// Example usage for scenarios with context:
        /// <code>
        /// builder.WithContext&lt;SpeditionContext&gt;().AddSteps(
        ///     _ => _.Given_there_is_an_active_customer_with_id("ABC-123"),
        ///     _ => _.Given_the_customer_has_product_in_basket("wooden shelf"),
        ///     _ => _.When_the_customer_payment_finalizes(),
        ///     _ => _.Then_product_should_be_dispatched_to_the_customer("wooden shelf"));
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
        /// <param name="builder">Scenario builder.</param>
        /// <param name="steps">Steps to add.</param>
        /// <returns><paramref name="builder"/> instance.</returns>
        public static IScenarioBuilder<TContext> AddSteps<TContext>(this IScenarioBuilder<TContext> builder, params Expression<Action<TContext>>[] steps)
        {
            return new ExtendedStepsBuilder<IScenarioBuilder<TContext>, TContext>(builder).AddSteps(steps);
        }

        /// <summary>
        /// Adds steps specified by <paramref name="steps"/> parameter.<br/>
        /// The steps would be executed in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
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
        /// builder.AddAsyncSteps(
        ///     _ => Given_product_is_available_in_product_storage("wooden desk"),
        ///     _ => When_customer_buys_product("wooden desk"),
        ///     _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62));
        /// </code>
        /// Expected step signature:
        /// <code>
        /// async Task Given_product_is_available_in_product_storage(string product) { /* ... */ }
        /// </code>
        /// </para>
        /// <para>
        /// Example usage for scenarios with context:
        /// <code>
        /// builder.WithContext&lt;SpeditionContext&gt;().AddAsyncSteps(
        ///     _ => _.Given_there_is_an_active_customer_with_id("ABC-123"),
        ///     _ => _.Given_the_customer_has_product_in_basket("wooden shelf"),
        ///     _ => _.When_the_customer_payment_finalizes(),
        ///     _ => _.Then_product_should_be_dispatched_to_the_customer("wooden shelf"));
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
        /// <param name="builder">Scenario builder.</param>
        /// <param name="steps">Steps to add.</param>
        /// <returns><paramref name="builder"/> instance.</returns>
        public static IScenarioBuilder<TContext> AddAsyncSteps<TContext>(this IScenarioBuilder<TContext> builder, params Expression<Func<TContext, Task>>[] steps)
        {
            return new ExtendedStepsBuilder<IScenarioBuilder<TContext>, TContext>(builder).AddSteps(steps);
        }
    }
}