#nullable enable
using System.Linq;
using System.Reflection;
using LightBDD.Framework.Parameters.ObjectTrees;
using LightBDD.Framework.Parameters.ObjectTrees.Mappers;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Parameters;

[TestFixture, FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class ObjectTreeBuilderOptions_tests
{
    private readonly ObjectTreeBuilderOptions _default = ObjectTreeBuilderOptions.Default;

    [Test]
    public void AddValueType_should_add_new_type()
    {
        var options = _default.AddValueType(typeof(object));
        var expected = _default.ValueTypes.Append(typeof(object)).OrderBy(x => x.Name).ToArray();
        var actual = options.ValueTypes.OrderBy(x => x.Name).ToArray();
        actual.ShouldBeEquivalentTo(expected);

        _default.ValueTypes.ShouldNotContain(typeof(object));
    }

    [Test]
    public void RemoveValueType_should_remove_existing_type()
    {
        var actual = _default.RemoveValueType(typeof(string));
        actual.ValueTypes.ShouldContain(typeof(MemberInfo));
        actual.ValueTypes.ShouldNotContain(typeof(string));

        _default.ValueTypes.ShouldContain(typeof(string));
    }

    [Test]
    public void AppendMapper_should_add_mapper_to_the_beginning_of_the_Mappers_collection()
    {
        var mapper = Mock.Of<ArrayMapper>();
        var actual = _default.AppendMapper(mapper);
        actual.Mappers.Count().ShouldBeGreaterThan(1);
        actual.Mappers.First().ShouldBe(mapper);

        _default.Mappers.ShouldNotContain(mapper);
    }

    [Test]
    public void ClearMappers_should_remove_all_mappers()
    {
        _default.ClearMappers().Mappers.ShouldBeEmpty();
        _default.Mappers.ShouldNotBeEmpty();
    }

    [Test]
    public void WithMaxDepth_should_set_MaxDepth()
    {
        _default.WithMaxDepth(55).MaxDepth.ShouldBe(55);
        _default.MaxDepth.ShouldNotBe(55);
    }
}