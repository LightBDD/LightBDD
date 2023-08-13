#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Implementation;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework
{
    /// <summary>
    /// Class representing composite step that consists of sub-step collection and optional context provider defining context instance that should be shared between all steps.
    /// If a step method returns instance of <see cref="CompositeStep"/>, the specified sub-steps will be included in step execution, making given parent step passing only if all are successful.<br/>
    /// Example usage:
    /// <code>
    /// CompositeStep Given_invoice_with_item()
    /// {
    ///     return CompositeStep.DefineNew()
    ///         .AddAsyncSteps(
    ///             _ => Given_product_is_available_in_product_storage("wooden desk"),
    ///             _ => When_customer_buys_product("wooden desk"),
    ///             _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62))
    ///         .Build();
    /// }
    /// </code>
    /// </summary>
    public class CompositeStep : CompositeStepResultDescriptor
    {
        internal CompositeStep(Resolvable<object?> contextDescriptor, IEnumerable<StepDescriptor> steps)
        : base(contextDescriptor, steps) { }

        /// <summary>
        /// Instantiates a new <see cref="ICompositeStepBuilder"/> allowing to build <see cref="CompositeStep"/> instance.<br/>
        /// Example usage:
        /// <code>
        /// CompositeStep Given_invoice_with_item()
        /// {
        ///     return CompositeStep.DefineNew()
        ///         .AddAsyncSteps(
        ///             _ => Given_product_is_available_in_product_storage("wooden desk"),
        ///             _ => When_customer_buys_product("wooden desk"),
        ///             _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62))
        ///         .Build();
        /// }
        /// </code>
        /// </summary>
        /// <returns><see cref="ICompositeStepBuilder"/> instance.</returns>
        public static ICompositeStepBuilder DefineNew()
        {
            return new CompositeStepBuilder();
        }
    }
}
