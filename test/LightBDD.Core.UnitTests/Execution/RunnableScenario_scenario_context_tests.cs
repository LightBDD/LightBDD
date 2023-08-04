using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class RunnableScenario_scenario_context_tests
{
    [Test]
    public async Task It_should_expose_scenario_context_for_scenario_method()
    {
        IScenarioInfo capturedInfo = null;
        Task ScenarioMethod(ICoreScenarioStepsRunner _)
        {
            var current = ScenarioExecutionContext.CurrentScenario;
            current.ShouldNotBeNull();
            current.Fixture.ShouldBeOfType<MyFixture>();
            capturedInfo = current.Info;
            current.DependencyResolver.ShouldNotBeNull();
            current.Context.ShouldNotBeNull();
            return Task.CompletedTask;
        }
        var result = await TestableScenarioFactory.Default.RunScenario<MyFixture>(ScenarioMethod);
        result.StatusDetails.ShouldBeNull();
        result.Status.ShouldBe(ExecutionStatus.Passed);
        capturedInfo.ShouldBe(result.Info);
    }

    [Test]
    public async Task It_should_use_scenario_DI_scope_for_resource_management()
    {
        void ConfigureDi(IDefaultContainerConfigurator container)
        {
            container.RegisterType<Disposable1>(InstanceScope.Single);
            container.RegisterType<Disposable2>(InstanceScope.Scenario);
        }

        Disposable1 capturedSingleton = null;
        Disposable2 capturedScoped = null;
        var factory = TestableScenarioFactory.Create(cfg => cfg.DependencyContainerConfiguration().UseDefault(ConfigureDi));
        await factory.RunScenario(_ =>
        {
            capturedSingleton = ScenarioExecutionContext.CurrentScenario.DependencyResolver.Resolve<Disposable1>();
            capturedScoped = ScenarioExecutionContext.CurrentScenario.DependencyResolver.Resolve<Disposable2>();

            capturedSingleton.Disposed.ShouldBe(false);
            capturedScoped.Disposed.ShouldBe(false);

            return Task.CompletedTask;
        });

        capturedSingleton.Disposed.ShouldBeFalse();
        capturedScoped.Disposed.ShouldBeTrue();
    }

    [Test]
    public async Task It_should_fail_scenario_on_DI_scope_disposal_failure()
    {
        void ConfigureDi(IDefaultContainerConfigurator container)
        {
            container.RegisterType<FaultyDisposable>(InstanceScope.Scenario);
        }

        var factory = TestableScenarioFactory.Create(cfg => cfg.DependencyContainerConfiguration().UseDefault(ConfigureDi));
        var result = await factory.RunScenario(_ =>
        {
            ScenarioExecutionContext.CurrentScenario.DependencyResolver.Resolve<FaultyDisposable>();
            return Task.CompletedTask;
        });

        result.Status.ShouldBe(ExecutionStatus.Failed);
        result.StatusDetails.ShouldBe("Scenario Failed: System.InvalidOperationException: DI Scope Dispose() failed: Failed to dispose dependency 'FaultyDisposable': I am faulty");
    }

    class Disposable1 : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }

    class Disposable2 : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }

    class FaultyDisposable : IDisposable
    {
        public void Dispose() => throw new Exception("I am faulty");
    }

    class MyFixture
    {
    }
}