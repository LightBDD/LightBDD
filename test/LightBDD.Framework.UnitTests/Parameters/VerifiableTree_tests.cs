using System;
using System.Collections.Generic;
using System.Dynamic;
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
        AssertNodes(tree.Details.Root.EnumerateAll(),
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

        AssertNodes(tree.Details.Root.EnumerateAll(),
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

        AssertNodes(tree.Details.Root.EnumerateAll(),
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

    [Test]
    public void Tree_should_order_details_using_names_for_properties_and_indexes_for_array_items_and_add_surplus_items_at_the_end()
    {
        var expected = new
        {
            Name = "Bob",
            Surname = "Johnson",
            Items = Enumerable.Range(0, 15)
        };
        var actual = new
        {
            Id = "X-11",
            Name = "Bob",
            Surname = "Johnson",
            Items = Enumerable.Range(0, 16)
        };
        var tree = new VerifiableTree(expected, new());
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Items|<array:15>|<array:16>|Failure|Different collection size",
            "$.Items[0]|0|0|Success|",
            "$.Items[1]|1|1|Success|",
            "$.Items[2]|2|2|Success|",
            "$.Items[3]|3|3|Success|",
            "$.Items[4]|4|4|Success|",
            "$.Items[5]|5|5|Success|",
            "$.Items[6]|6|6|Success|",
            "$.Items[7]|7|7|Success|",
            "$.Items[8]|8|8|Success|",
            "$.Items[9]|9|9|Success|",
            "$.Items[10]|10|10|Success|",
            "$.Items[11]|11|11|Success|",
            "$.Items[12]|12|12|Success|",
            "$.Items[13]|13|13|Success|",
            "$.Items[14]|14|14|Success|",
            "$.Items[15]|<none>|15|Failure|Unexpected value",
            "$.Name|Bob|Bob|Success|",
            "$.Surname|Johnson|Johnson|Success|",
            "$.Id|<none>|X-11|Failure|Unexpected value"
        );
    }

    [Test]
    public void It_should_compare_class_with_expando()
    {
        var expected = new { Name = "Bob", Surname = "Johnson", Items = new[] { 0, 1, 2 } };

        dynamic actual = new ExpandoObject();
        actual.Name = "Bob";
        actual.Surname = "Johnson";
        actual.Items = new[] { 0, 1, 2 };

        var tree = new VerifiableTree(expected, new());
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Items|<array:3>|<array:3>|Success|",
            "$.Items[0]|0|0|Success|",
            "$.Items[1]|1|1|Success|",
            "$.Items[2]|2|2|Success|",
            "$.Name|Bob|Bob|Success|",
            "$.Surname|Johnson|Johnson|Success|"
        );
    }

    [Test]
    public void It_should_compare_reference_nodes()
    {
        var expected = new[]
        {
            new Node{Name = "P1",Children = { new (){Name = "C1"} }},
            new Node{Name = "P2",Children = { new (){Name = "C2", Children = { new(){Name = "CC1"} }} }}
        };
        expected[0].Children[0].Parent = expected[0];
        expected[1].Children[0].Parent = expected[1];
        expected[1].Children[0].Children[0].Parent = expected[1].Children[0];

        var actual = new[]
        {
            new Node{Name = "P3",Children = { new (){Name = "C1"} }},
            new Node{Name = "P2",Children = { new (){Name = "C2", Children = { new(){Name = "CC1"} }} }}
        };
        actual[0].Children[0].Parent = actual[0];
        actual[1].Children[0].Parent = actual[1];
        actual[1].Children[0].Children[0].Parent = actual[1];

        var tree = new VerifiableTree(expected, new());
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<array:2>|<array:2>|Success|", 
            "$[0]|<object>|<object>|Success|",
            "$[0].Children|<array:1>|<array:1>|Success|", 
            "$[0].Children[0]|<object>|<object>|Success|",
            "$[0].Children[0].Children|<array:0>|<array:0>|Success|", 
            "$[0].Children[0].Name|C1|C1|Success|",
            "$[0].Children[0].Parent|<ref: $[0]>|<ref: $[0]>|Success|",
            "$[0].Name|P1|P3|Failure|expected: equals 'P1', but got: 'P3'", 
            "$[0].Parent|<null>|<null>|Success|",
            "$[1]|<object>|<object>|Success|", 
            "$[1].Children|<array:1>|<array:1>|Success|",
            "$[1].Children[0]|<object>|<object>|Success|", 
            "$[1].Children[0].Children|<array:1>|<array:1>|Success|",
            "$[1].Children[0].Children[0]|<object>|<object>|Success|",
            "$[1].Children[0].Children[0].Children|<array:0>|<array:0>|Success|",
            "$[1].Children[0].Children[0].Name|CC1|CC1|Success|",
            "$[1].Children[0].Children[0].Parent|<ref: $[1].Children[0]>|<ref: $[1]>|Failure|expected: equals '$[1].Children[0]', but got: '$[1]'",
            "$[1].Children[0].Name|C2|C2|Success|", 
            "$[1].Children[0].Parent|<ref: $[1]>|<ref: $[1]>|Success|",
            "$[1].Name|P2|P2|Success|", 
            "$[1].Parent|<null>|<null>|Success|"
        );
    }

    private void AssertNodes(IEnumerable<ITreeParameterNodeResult> nodes, params string[] expected)
    {
        var actual = nodes.Select(n => $"{n.Path}|{n.Expectation}|{n.Value}|{n.VerificationStatus}|{n.VerificationMessage}").ToArray();
        actual.ShouldBe(expected);
    }

    class Node
    {
        public Node? Parent { get; set; }
        public string Name { get; set; }
        public List<Node> Children { get; } = new();
    }
}