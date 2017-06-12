using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework
{
    /// <summary>
    /// Class representing step group that consists of sub-step collection and context provider defining context instance that should be shared between all steps.
    /// If a step method returns instance of <see cref="StepGroup"/>, the specified sub-steps will be included in step execution, making given parent step passing only if all are successful.<br/>
    /// Example usage:
    /// <code>
    /// StepGroup Given_invoice_with_product()
    /// {
    ///     return StepGroup.DefineNew()
    ///         .AddSteps(
    ///             _ => Given_product_is_available_in_product_storage("wooden desk"),
    ///             _ => When_customer_buys_product("wooden desk"),
    ///             _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62))
    ///         .Build();
    /// }
    /// </code>
    /// </summary>
    [DebuggerStepThrough]
    public class StepGroup : CompositeStepResultDescriptor
    {
        internal StepGroup(Func<object> contextProvider, IEnumerable<StepDescriptor> steps)
        : base(contextProvider, steps) { }

        /// <summary>
        /// Instantiates a new <see cref="IStepGroupBuilder"/> allowing to build <see cref="StepGroup"/> instance.<br/>
        /// Example usage:
        /// <code>
        /// StepGroup Given_invoice_with_product()
        /// {
        ///     return StepGroup.DefineNew()
        ///         .AddSteps(
        ///             _ => Given_product_is_available_in_product_storage("wooden desk"),
        ///             _ => When_customer_buys_product("wooden desk"),
        ///             _ => Then_invoice_should_contain_product_with_price_of_AMOUNT_pounds("wooden desk", 62))
        ///         .Build();
        /// }
        /// </code>
        /// </summary>
        /// <returns><see cref="IStepGroupBuilder"/> instance.</returns>
        public static IStepGroupBuilder DefineNew() => new StepGroupBuilder();
    }
}
