using System;
using Xunit;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// An EXPERIMENTAL attribute allowing to control inter-class test parallelization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ClassCollectionBehaviorAttribute : Attribute
    {
        /// <summary>
        /// If true, the class tests as well as test cases will run in parallel as long as following criteria are met: <br/>
        /// * test class does not implement <see cref="IClassFixture{TFixture}"/> interface,<br/>
        /// * test class does not implement <see cref="ICollectionFixture{TFixture}"/> interface,<br/>
        /// * test class does not have <see cref="CollectionAttribute"/> attribute applied.<br/>
        ///
        /// The test parallelization honors the <see cref="CollectionBehaviorAttribute"/> and runner settings such as <see cref="CollectionBehaviorAttribute.DisableTestParallelization"/> or <see cref="CollectionBehaviorAttribute.MaxParallelThreads"/>. <br/>
        /// The <see cref="CollectionBehavior.CollectionPerAssembly"/> setting is currently not honored.
        /// </summary>
        public bool AllowTestParallelization { get; set; }
    }
}
