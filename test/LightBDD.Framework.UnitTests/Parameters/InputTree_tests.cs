using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;
using LightBDD.Framework.Parameters;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Parameters;

[TestFixture, FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class InputTree_tests
{
    [Test]
    public void It_should_represent_input_object_as_a_tree()
    {
        var input = new
        {
            Name = "Bob",
            Surname = "Johnson",
            Items = new object[] { 3.14, false },
            Inner = new { Key = 5, Value = 'c' }
        };
        var tree = Tree.For(input);
        tree.Input.ShouldBeSameAs(input);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|NotApplicable|",
            "$.Inner|<object>|<object>|NotApplicable|",
            "$.Inner.Key|5|5|NotApplicable|",
            "$.Inner.Value|c|c|NotApplicable|",
            "$.Items|<array:2>|<array:2>|NotApplicable|",
            "$.Items[0]|3.14|3.14|NotApplicable|",
            "$.Items[1]|False|False|NotApplicable|",
            "$.Name|Bob|Bob|NotApplicable|",
            "$.Surname|Johnson|Johnson|NotApplicable|"
        );

        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.NotApplicable);
        tree.Details.VerificationMessage.ShouldBe(null);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void It_should_exclude_null_properties_when_specified(bool excludeNullProperties)
    {
        var input = new
        {
            Name = "Bob",
            Value = (string)null
        };
        var tree = Tree.For(input, new() { ExcludeNullProperties = excludeNullProperties });
        tree.Input.ShouldBeSameAs(input);

        var nodes = tree.Details.Root.EnumerateAll().ToDictionary(x => x.Path);
        nodes.ShouldContainKey("$");
        nodes.ShouldContainKey("$.Name");
        nodes.ContainsKey("$.Value").ShouldBe(!excludeNullProperties);
    }

    private void AssertNodes(IEnumerable<ITreeParameterNodeResult> nodes, params string[] expected)
    {
        var actual = nodes.Select(n => $"{n.Path}|{n.Expectation}|{n.Value}|{n.VerificationStatus}|{n.VerificationMessage}").ToArray();
        actual.ShouldBe(expected);
    }
}