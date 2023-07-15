﻿using System;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.Runner.Implementation;
using Xunit.Abstractions;

namespace LightBDD.Runner
{
    /// <summary>
    /// Base class for feature tests with XUnit framework.
    /// It offers <see cref="Runner"/> property allowing to execute scenarios belonging to the feature class.
    /// </summary>
    public class FeatureFixture : ITestOutputProvider
    {
        private readonly ITestOutputHelper _testOutput;

        /// <summary>
        /// Returns <see cref="ITestOutputHelper"/> associated to the test class instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when TestOutput is null (scenario was executed from test without [Scenario] attribute or was explicitly initialized with null).</exception>
        public ITestOutputHelper TestOutput => _testOutput ?? throw new InvalidOperationException(nameof(TestOutput) + " is not provided. Ensure that scenario is executed from method with [Scenario] attribute, or " + nameof(ITestOutputHelper) + " instance is provided to " + nameof(FeatureFixture) + " constructor.");

        /// <summary>
        /// Returns <see cref="IBddRunner"/> allowing to execute scenarios belonging to the feature class.
        /// </summary>
        protected IBddRunner Runner { get; }

        /// <summary>
        /// Default constructor initializing <see cref="Runner"/> for feature class instance and configures <see cref="TestOutput"/> with default test output.
        /// </summary>
        protected FeatureFixture() : this(TestContextProvider.Current?.OutputHelper)
        {
        }

        /// <summary>
        /// Constructor initializing <see cref="Runner"/> for feature class instance as well as configures <see cref="TestOutput"/> with <paramref name="output"/>.
        /// </summary>
        protected FeatureFixture(ITestOutputHelper output)
        {
            _testOutput = output;
            Runner = XUnit2FeatureCoordinator.GetInstance().RunnerRepository.GetRunnerFor(GetType()).GetBddRunner(this);
        }
    }
}