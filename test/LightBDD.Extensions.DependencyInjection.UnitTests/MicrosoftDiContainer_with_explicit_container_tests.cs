using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LightBDD.Extensions.DependencyInjection.UnitTests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class MicrosoftDiContainer_with_explicit_container_tests : ContainerBaseTests
    {
        public MicrosoftDiContainer_with_explicit_container_tests(bool shouldTakeOwnership)
            : base(shouldTakeOwnership)
        {
        }

        protected override IDependencyContainer CreateContainer()
        {
            return CreateContainer(
                services => services.AddTransient<Disposable>().AddSingleton<DisposableSingleton>(),
                opt => opt.TakeOwnership(ShouldTakeOwnership).EnableScopeNestingWithinScenarios(true));
        }

        private IDependencyContainer CreateContainer(Action<ServiceCollection> configure, Action<DiContainerOptions> options)
        {
            var serviceCollection = new ServiceCollection();
            configure?.Invoke(serviceCollection);

            return new DependencyContainerConfiguration()
                .UseContainer(serviceCollection.BuildServiceProvider(), options)
                .Build();
        }

        [Test]
        public void BeginScope_should_use_nested_scenario_scopes_if_enabled()
        {
            Disposable1 singleton;
            using (var container = CreateContainer(services => services
                    .AddTransient<Disposable>()
                    .AddSingleton<Disposable1>()
                    .AddScoped<Disposable2>(),
                opt => opt.EnableScopeNestingWithinScenarios(true)))
            {
                singleton = container.Resolve<Disposable1>();

                using (var scenario = container.BeginScope())
                {
                    Assert.AreSame(singleton, scenario.Resolve<Disposable1>());
                    Assert.AreNotSame(container.Resolve<Disposable>(), scenario.Resolve<Disposable>());
                    Assert.AreNotSame(container.Resolve<Disposable2>(), scenario.Resolve<Disposable2>());

                    using (var scopeA = scenario.BeginScope())
                    {
                        Assert.AreSame(singleton, scopeA.Resolve<Disposable1>());
                        Assert.AreNotSame(scenario.Resolve<Disposable>(), scopeA.Resolve<Disposable>());
                        Assert.AreNotSame(scenario.Resolve<Disposable2>(), scopeA.Resolve<Disposable2>());

                        Disposable scopeBDisposable;
                        Disposable2 scopeBDisposable2;
                        using (var scopeB = scopeA.BeginScope())
                        {
                            Assert.AreSame(singleton, scopeB.Resolve<Disposable1>());

                            scopeBDisposable = scopeB.Resolve<Disposable>();
                            scopeBDisposable2 = scopeB.Resolve<Disposable2>();
                            Assert.AreNotSame(scopeA.Resolve<Disposable>(), scopeBDisposable);
                            Assert.AreNotSame(scopeA.Resolve<Disposable2>(), scopeBDisposable2);
                        }

                        Assert.True(scopeBDisposable.Disposed);
                        Assert.True(scopeBDisposable2.Disposed);
                    }
                }
                Assert.False(singleton.Disposed);
            }
            Assert.True(singleton.Disposed);
        }

        [Test]
        public void BeginScope_should_use_single_scenario_scope_if_nested_scopes_are_disabled()
        {
            Disposable1 singleton;
            using (var container = CreateContainer(services => services
                    .AddTransient<Disposable>()
                    .AddSingleton<Disposable1>()
                    .AddScoped<Disposable2>(),
                opt => opt.EnableScopeNestingWithinScenarios(false)))
            {
                singleton = container.Resolve<Disposable1>();
                Disposable scopeBDisposable;
                Disposable2 scopeBDisposable2;

                using (var scenario = container.BeginScope())
                using (var scenario2 = container.BeginScope())
                {
                    Assert.AreSame(singleton, scenario.Resolve<Disposable1>());
                    Assert.AreNotSame(container.Resolve<Disposable>(), scenario.Resolve<Disposable>());
                    Assert.AreNotSame(container.Resolve<Disposable2>(), scenario.Resolve<Disposable2>());
                    Assert.AreNotSame(scenario.Resolve<Disposable2>(), scenario2.Resolve<Disposable2>());

                    using (var scopeA = scenario.BeginScope())
                    {
                        Assert.AreSame(singleton, scopeA.Resolve<Disposable1>());
                        Assert.AreNotSame(scenario.Resolve<Disposable>(), scopeA.Resolve<Disposable>());
                        Assert.AreSame(scenario.Resolve<Disposable2>(), scopeA.Resolve<Disposable2>());

                        using (var scopeB = scopeA.BeginScope())
                        {
                            Assert.AreSame(singleton, scopeB.Resolve<Disposable1>());

                            scopeBDisposable = scopeB.Resolve<Disposable>();
                            scopeBDisposable2 = scopeB.Resolve<Disposable2>();
                            Assert.AreNotSame(scopeA.Resolve<Disposable>(), scopeBDisposable);
                            Assert.AreSame(scopeA.Resolve<Disposable2>(), scopeBDisposable2);
                        }

                        Assert.False(scopeBDisposable.Disposed);
                        Assert.False(scopeBDisposable2.Disposed);
                    }
                    Assert.False(scopeBDisposable.Disposed);
                    Assert.False(scopeBDisposable2.Disposed);
                }
                Assert.True(scopeBDisposable.Disposed);
                Assert.True(scopeBDisposable2.Disposed);
                Assert.False(singleton.Disposed);
            }
            Assert.True(singleton.Disposed);
        }
    }
}