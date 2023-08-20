﻿using System;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_hierarchical_execution_tests
    {
        private ICompositeStepBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new TestableCompositeStepBuilder();
        }

        [Test]
        public void It_should_allow_to_apply_context_instance()
        {
            var context = new object();
            var stepGroup = _builder.WithContext(context).Build();
            var instance = ResolveInstance(stepGroup);
            Assert.That(instance, Is.SameAs(context));
        }

        [Test]
        public void It_should_allow_to_apply_context_provider()
        {
            var stepGroup = _builder.WithContext(() => TimeSpan.FromSeconds(5)).Build();
            var instance = ResolveInstance(stepGroup);
            Assert.That(instance, Is.EqualTo(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public void It_should_allow_to_apply_context_with_parameterless_constructor()
        {
            var stepGroup = _builder.WithContext<MyContext>().Build();
            var instance = ResolveInstance(stepGroup);
            Assert.That(instance, Is.InstanceOf<MyContext>());
        }

        [Test]
        public void It_should_not_allow_defining_context_multiple_times()
        {
            var ctx = new object();
            _builder.WithContext(ctx);
            var ex = Assert.Throws<InvalidOperationException>(() => _builder.WithContext(ctx));
            Assert.That(ex.Message, Is.EqualTo("Step context can be specified only once, when no steps are specified yet."));
        }

        [Test]
        public void It_should_not_allow_defining_context_after_some_steps_are_added()
        {
            var ctx = new object();
            _builder.AddSteps(() => { });
            var ex = Assert.Throws<InvalidOperationException>(() => _builder.WithContext(ctx));
            Assert.That(ex.Message, Is.EqualTo("Step context can be specified only once, when no steps are specified yet."));
        }

        [Test]
        public void It_should_allow_configuring_context_on_instantiation()
        {
            var stepGroup = _builder.WithContext<ConfigurableContext>(ctx => ctx.Text = "foo")
                .Build();
            var instance = (ConfigurableContext)ResolveInstance(stepGroup);
            Assert.AreEqual("foo", instance.Text);
        }

        [Test]
        public void It_should_allow_configuring_context_on_instantiation_when_using_resolver()
        {
            var stepGroup = _builder.WithContext(
                    r => r.Resolve<ConfigurableContext>(),
                    ctx => ctx.Text = "foo")
                .Build();
            var instance = (ConfigurableContext)ResolveInstance(stepGroup);
            Assert.AreEqual("foo", instance.Text);
        }

        class ConfigurableContext
        {
            public string Text { get; set; }
        }

        private static object ResolveInstance(CompositeStep stepGroup)
        {
            //TODO: rework
            var container = new LightBddConfiguration().BuildContainer().BeginScope();
            return stepGroup.SubStepsContext.ContextResolver(container);
        }
    }
}