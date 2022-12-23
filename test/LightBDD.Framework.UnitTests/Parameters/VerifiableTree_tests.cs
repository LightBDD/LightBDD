using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;
using LightBDD.Framework.Parameters;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Parameters;

[TestFixture, FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class VerifiableTree_tests
{
    [Test]
    public void Tree_should_return_NotProvided_result_when_SetActual_is_not_called()
    {
        var input = new { Name = "Bob", Surname = "Johnson" };
        var tree = new VerifiableTree(input, new());

        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.NotProvided);
        tree.Details.VerificationMessage.ShouldBe("Actual value was not provided");
        AssertNodes(tree.Details.Nodes,
            "$|<object>|<none>|Failure|Missing value",
            "$.Name|Bob|<none>|Failure|Missing value",
            "$.Surname|Johnson|<none>|Failure|Missing value"
        );
    }

    [Test]
    public void Tree_should_match_two_structurally_equal_objects()
    {
        var expected = new
        {
            Name = "Bob",
            Surname = "Johnson",
            Items = new object[] { 3.14, false },
            Inner = new { Key = 5, Value = 'c' }
        };
        var actual = new
        {
            Name = "Bob",
            Surname = "Johnson",
            Items = new List<object> { 3.14, false },
            Inner = new KeyValuePair<int, char>(5, 'c')
        };

        var tree = new VerifiableTree(expected, new VerifiableTreeOptions());
        tree.SetActual(actual);

        AssertNodes(tree.Details.Nodes,
            "$|<object>|<object>|Success|",
            "$.Inner|<object>|<object>|Success|",
            "$.Inner.Key|5|5|Success|",
            "$.Inner.Value|c|c|Success|",
            "$.Items|<array:2>|<array:2>|Success|",
            "$.Items[0]|3.14|3.14|Success|",
            "$.Items[1]|False|False|Success|",
            "$.Name|Bob|Bob|Success|",
            "$.Surname|Johnson|Johnson|Success|"
        );
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Success);
        tree.Details.VerificationMessage.ShouldBeNull();
    }

    [Test]
    public void Tree_should_fail_on_unequal_objects()
    {
        var expected = new
        {
            Name = "Bob",
            Surname = "Johnson",
            Items = new object[] { 3.14, false },
            Inner = new { Key = 5, Value = 'c' }
        };
        var actual = new
        {
            Surname = "John",
            Items = new List<object> { 3.14, false, 5 },
            Inner = 'c'
        };

        var tree = new VerifiableTree(expected, new VerifiableTreeOptions());
        tree.SetActual(actual);

        AssertNodes(tree.Details.Nodes,
            "$|<object>|<object>|Success|", 
            "$.Inner|<object>|c|Failure|Different node types",
            "$.Inner.Key|5|<none>|Failure|Missing value", 
            "$.Inner.Value|c|<none>|Failure|Missing value",
            "$.Items|<array:2>|<array:3>|Failure|Different collection size", 
            "$.Items[0]|3.14|3.14|Success|",
            "$.Items[1]|False|False|Success|", 
            "$.Items[2]|<none>|5|Failure|Unexpected value",
            "$.Name|Bob|<none>|Failure|Missing value",
            "$.Surname|Johnson|John|Failure|expected: equals 'Johnson', but got: 'John'"
        );
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Failure);
        tree.Details.VerificationMessage.ShouldBe(@"$.Inner: Different node types
$.Inner.Key: Missing value
$.Inner.Value: Missing value
$.Items: Different collection size
$.Items[2]: Unexpected value
$.Name: Missing value
$.Surname: expected: equals 'Johnson', but got: 'John'");
    }

    private void AssertNodes(IReadOnlyList<ITreeParameterNodeResult> nodes, params string[] expected)
    {
        var actual = nodes.Select(n => $"{n.Path}|{n.Expectation}|{n.Value}|{n.VerificationStatus}|{n.VerificationMessage}").ToArray();
        actual.ShouldBe(expected);
    }
}