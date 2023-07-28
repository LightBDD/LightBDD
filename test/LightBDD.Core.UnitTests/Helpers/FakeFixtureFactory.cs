using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.UnitTests.Helpers;

class FakeFixtureFactory : IFixtureFactory
{
    private readonly object _fixture;

    public FakeFixtureFactory(object fixture)
    {
        _fixture = fixture;
    }

    public object Create(Type fixtureType) => _fixture;
}