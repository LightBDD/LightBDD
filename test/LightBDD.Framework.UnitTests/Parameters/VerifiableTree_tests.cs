using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using LightBDD.ScenarioHelpers;
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
        var tree = Tree.ExpectEquivalent(input);

        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.NotProvided);
        tree.Details.VerificationMessage.ShouldBe("Actual value was not provided");
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<none>|Failure|Missing value",
            "$.Name|Bob|<none>|Failure|Missing value",
            "$.Surname|Johnson|<none>|Failure|Missing value"
        );
        tree.Expected.ShouldBeSameAs(input);
        var treeBase = (VerifiableTree)tree;
        treeBase.Expected.ShouldBeSameAs(input);
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

        var tree = Tree.ExpectEquivalent(expected);
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

        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Inner|<object>|c|Failure|Different node types",
            "$.Inner.Key|5|<none>|Failure|Missing value",
            "$.Inner.Value|c|<none>|Failure|Missing value",
            "$.Items|<array:2>|<array:3>|Failure|Expected exactly 2 items",
            "$.Items[0]|3.14|3.14|Success|",
            "$.Items[1]|False|False|Success|",
            "$.Items[2]|<none>|5|Failure|Unexpected value",
            "$.Name|Bob|<none>|Failure|Missing value",
            "$.Surname|Johnson|John|Failure|expected: equals 'Johnson', but got: 'John'"
        );

        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Failure);
        tree.Details.VerificationMessage.NormalizeNewLine().ShouldBe(@"$.Inner: Different node types
$.Inner.Key: Missing value
$.Inner.Value: Missing value
$.Items: Expected exactly 2 items
$.Items[2]: Unexpected value
$.Name: Missing value
$.Surname: expected: equals 'Johnson', but got: 'John'".NormalizeNewLine());
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
        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Items|<array:15>|<array:16>|Failure|Expected exactly 15 items",
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

        var tree = Tree.ExpectEquivalent(expected);
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

        var tree = Tree.ExpectEquivalent(expected);
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

    [Test]
    public void It_should_compare_JsonElement_to_model()
    {
        var json = @"{
    ""Name"":""John"",
    ""Surname"":""Smith"",
    ""Items"":[1,2,3,3.14,true,false,null],
    ""Inner"":{""Label"":""some text""}
}";
        var expected = new
        {
            Name = "John",
            Surname = "Smith",
            Items = new object[] { 1, 2, 3, 3.14, true, false, null },
            Inner = new { Label = "some text" }
        };
        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(JsonDocument.Parse(json).RootElement);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Inner|<object>|<object>|Success|",
            "$.Inner.Label|some text|some text|Success|",
            "$.Items|<array:7>|<array:7>|Success|",
            "$.Items[0]|1|1|Success|",
            "$.Items[1]|2|2|Success|",
            "$.Items[2]|3|3|Success|",
            "$.Items[3]|3.14|3.14|Success|",
            "$.Items[4]|True|True|Success|",
            "$.Items[5]|False|False|Success|",
            "$.Items[6]|<null>|<null>|Success|",
            "$.Name|John|John|Success|",
            "$.Surname|Smith|Smith|Success|"
        );
    }

    [Test]
    public void It_should_compare_JsonElement_to_Newtonsoft_expando()
    {
        var json = @"{
    ""Name"":""John"",
    ""Surname"":""Smith"",
    ""Items"":[1,2,3,3.14,true,false,null],
    ""Inner"":{""Label"":""some text""}
}";
        var expected = JsonDocument.Parse(json).RootElement;
        var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(json);
        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Inner|<object>|<object>|Success|",
            "$.Inner.Label|some text|some text|Success|",
            "$.Items|<array:7>|<array:7>|Success|",
            "$.Items[0]|1|1|Success|",
            "$.Items[1]|2|2|Success|",
            "$.Items[2]|3|3|Success|",
            "$.Items[3]|3.14|3.14|Success|",
            "$.Items[4]|True|True|Success|",
            "$.Items[5]|False|False|Success|",
            "$.Items[6]|<null>|<null>|Success|",
            "$.Name|John|John|Success|",
            "$.Surname|Smith|Smith|Success|"
        );
    }

    [Test]
    public void It_should_compare_objects_with_compatible_number_types()
    {
        var expected = new object[] { (byte)1, (short)2, 3.2m };
        var actual = new object[] { 1u, 2ul, 3.2f };

        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<array:3>|<array:3>|Success|",
            "$[0]|1|1|Success|",
            "$[1]|2|2|Success|",
            "$[2]|3.2|3.2|Success|"
        );
    }

    [Test]
    public void It_should_successfully_match_object_against_specified_expectations()
    {
        var expected = new
        {
            Name = Expect.To.Not.BeEmpty<string>(),
            Surname = Expect.To.Not.BeEmpty<string>(),
            Age = Expect.To.BeGreaterOrEqual(0).And(x => x.BeLessOrEqual(120))
        };

        var actual = new { Name = "Bob", Surname = "Smith", Age = 23L };
        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Age|(greater or equal '0' and less or equal '120')|23|Success|",
            "$.Name|not empty|Bob|Success|",
            "$.Surname|not empty|Smith|Success|"
        );
    }

    [Test]
    public void ExpectContaining_should_successfully_match_objects()
    {
        var expected = new
        {
            Name = "Bob",
            Items = new[] { 1, 2, 3 }
        };
        var actual = new
        {
            Name = "Bob",
            Surname = "Johnson",
            Items = new[] { 1, 2, 3, 4 }
        };

        var tree = Tree.ExpectContaining(expected);
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Items|<array:3>|<array:4>|Success|",
            "$.Items[0]|1|1|Success|",
            "$.Items[1]|2|2|Success|",
            "$.Items[2]|3|3|Success|",
            "$.Name|Bob|Bob|Success|");
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Success);
    }

    [Test]
    public void ExpectContaining_should_fail_if_actual_object_does_not_have_all_expected_values()
    {
        var expected = new
        {
            Name = "Bob",
            Items = new[] { 1, 2, 3 }
        };
        var actual = new
        {
            Surname = "Johnson",
            Items = new[] { 1, 2, }
        };

        var tree = Tree.ExpectContaining(expected);
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Items|<array:3>|<array:2>|Failure|Expected at least 3 items",
            "$.Items[0]|1|1|Success|",
            "$.Items[1]|2|2|Success|",
            "$.Items[2]|3|<none>|Failure|Missing value",
            "$.Name|Bob|<none>|Failure|Missing value");
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Failure);
    }

    [Test]
    public void ExpectAtLeastContaining_should_successfully_match_objects_and_include_surplus_values()
    {
        var expected = new
        {
            Name = "Bob",
            Items = new[] { 1, 2, 3 }
        };
        var actual = new
        {
            Name = "Bob",
            Surname = "Johnson",
            Items = new[] { 1, 2, 3, 4 }
        };

        var tree = Tree.ExpectAtLeastContaining(expected);
        tree.SetActual(actual);
        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Items|<array:3>|<array:4>|Success|",
            "$.Items[0]|1|1|Success|",
            "$.Items[1]|2|2|Success|",
            "$.Items[2]|3|3|Success|",
            "$.Items[3]|<none>|4|NotApplicable|Surplus value",
            "$.Name|Bob|Bob|Success|",
            "$.Surname|<none>|Johnson|NotApplicable|Surplus value");
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Success);
    }

    [Test]
    public void ExpectEquivalent_should_match_two_structurally_equal_objects()
    {
        var expected = new
        {
            Items = new object[] { 3.14, false },
            Inner = new { Key = 5, Value = 'c' }
        };
        var actual = new
        {
            Items = new List<object> { 3.14, false },
            Inner = new KeyValuePair<int, char>(5, 'c')
        };

        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Inner|<object>|<object>|Success|",
            "$.Inner.Key|5|5|Success|",
            "$.Inner.Value|c|c|Success|",
            "$.Items|<array:2>|<array:2>|Success|",
            "$.Items[0]|3.14|3.14|Success|",
            "$.Items[1]|False|False|Success|"
        );
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Success);
        tree.Details.VerificationMessage.ShouldBeNull();
    }

    [Test]
    public void ExpectStrictMatch_should_match_two_objects_with_same_type_structure_and_equal_values()
    {
        var expected = new
        {
            Items = new object[] { 3.14, false },
            Inner = new { Key = 5, Value = 'c' }
        };
        var actual = new
        {
            Items = new object[] { 3.14, false },
            Inner = new { Key = 5, Value = 'c' }
        };

        var tree = Tree.ExpectStrictMatch(expected);
        tree.SetActual(actual);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Inner|<object>|<object>|Success|",
            "$.Inner.Key|5|5|Success|",
            "$.Inner.Value|c|c|Success|",
            "$.Items|<array:2>|<array:2>|Success|",
            "$.Items[0]|3.14|3.14|Success|",
            "$.Items[1]|False|False|Success|"
        );
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Success);
        tree.Details.VerificationMessage.ShouldBeNull();
    }

    [Test]
    public void ExpectStrictMatch_should_fail_objects_with_different_type_structure()
    {
        var expected = new
        {
            Items = (IEnumerable<object>)new List<object> { 3.14, false },
            Inner = (object)new KeyValuePair<int, char>(5, 'c')
        };
        var actual = new
        {
            Items = (IEnumerable<object>)new object[] { 3.14f, false },
            Inner = (object)new KeyValuePair<long, char>(5L, 'c')
        };

        var tree = Tree.ExpectStrictMatch(expected);
        tree.SetActual(actual);

        AssertNodes(tree.Details.Root.EnumerateAll(),
            "$|<object>|<object>|Success|",
            "$.Inner|<object>|<object>|Failure|expected 'System.Collections.Generic.KeyValuePair`2[System.Int32,System.Char]' type, but got 'System.Collections.Generic.KeyValuePair`2[System.Int64,System.Char]'",
            "$.Inner.Key|5|5|Failure|expected 'System.Int32' type, but got 'System.Int64'",
            "$.Inner.Value|c|c|Success|",
            "$.Items|<array:2>|<array:2>|Failure|expected 'System.Collections.Generic.List`1[System.Object]' type, but got 'System.Object[]'",
            "$.Items[0]|3.14|3.14|Failure|expected 'System.Double' type, but got 'System.Single'",
            "$.Items[1]|False|False|Success|"
        );
        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Failure);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Expect_should_support_type_check_for_object_nodes(bool checkObjectNodeTypes)
    {
        var expected = new
        {
            Items = (IEnumerable<object>)new object[] { 5L },
            Inner = (object)new KeyValuePair<int, char>(5, 'c')
        };
        var actual = new
        {
            Items = (IEnumerable<object>)new List<object> { 5 },
            Inner = (object)new KeyValuePair<object, char>(5, 'c')
        };

        var tree = Tree.Expect(expected, VerifiableTreeOptions.EquivalentMatch.WithCheckObjectNodeTypes(checkObjectNodeTypes));
        tree.SetActual(actual);

        var nodes = DumpNodes(tree.Details.Root.EnumerateAll());
        nodes.ShouldContain("$|<object>|<object>|Success|");
        if (checkObjectNodeTypes)
            nodes.ShouldContain("$.Inner|<object>|<object>|Failure|expected 'System.Collections.Generic.KeyValuePair`2[System.Int32,System.Char]' type, but got 'System.Collections.Generic.KeyValuePair`2[System.Object,System.Char]'");
        else
            nodes.ShouldContain("$.Inner|<object>|<object>|Success|");
        nodes.ShouldContain("$.Inner.Key|5|5|Success|");
        nodes.ShouldContain("$.Inner.Value|c|c|Success|");
        nodes.ShouldContain("$.Items|<array:1>|<array:1>|Success|");
        nodes.ShouldContain("$.Items[0]|5|5|Success|");
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Expect_should_support_type_check_for_array_nodes(bool checkArrayNodeTypes)
    {
        var expected = new
        {
            Items = new object[] { 5L },
            Inner = new KeyValuePair<int, char>(5, 'c')
        };
        var actual = new
        {
            Items = new List<object> { 5 },
            Inner = new KeyValuePair<object, char>(5, 'c')
        };

        var tree = Tree.Expect(expected, VerifiableTreeOptions.EquivalentMatch.WithCheckArrayNodeTypes(checkArrayNodeTypes));
        tree.SetActual(actual);

        var nodes = DumpNodes(tree.Details.Root.EnumerateAll());
        nodes.ShouldContain("$|<object>|<object>|Success|");
        nodes.ShouldContain("$.Inner|<object>|<object>|Success|");
        nodes.ShouldContain("$.Inner.Key|5|5|Success|");
        nodes.ShouldContain("$.Inner.Value|c|c|Success|");
        if (checkArrayNodeTypes)
            nodes.ShouldContain("$.Items|<array:1>|<array:1>|Failure|expected 'System.Object[]' type, but got 'System.Collections.Generic.List`1[System.Object]'");
        else
            nodes.ShouldContain("$.Items|<array:1>|<array:1>|Success|");
        nodes.ShouldContain("$.Items[0]|5|5|Success|");
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Expect_should_support_type_check_for_value_nodes(bool checkValueNodeTypes)
    {
        var expected = new
        {
            Items = new object[] { 5L },
            Inner = new KeyValuePair<int, char>(5, 'c')
        };
        var actual = new
        {
            Items = new List<object> { 5 },
            Inner = new KeyValuePair<object, char>(5, 'c')
        };

        var tree = Tree.Expect(expected, VerifiableTreeOptions.EquivalentMatch.WithCheckValueNodeTypes(checkValueNodeTypes));
        tree.SetActual(actual);

        var nodes = DumpNodes(tree.Details.Root.EnumerateAll());
        nodes.ShouldContain("$|<object>|<object>|Success|");
        nodes.ShouldContain("$.Inner|<object>|<object>|Success|");
        nodes.ShouldContain("$.Inner.Key|5|5|Success|");
        nodes.ShouldContain("$.Inner.Value|c|c|Success|");
        nodes.ShouldContain("$.Items|<array:1>|<array:1>|Success|");
        if (checkValueNodeTypes)
            nodes.ShouldContain("$.Items[0]|5|5|Failure|expected 'System.Int64' type, but got 'System.Int32'");
        else
            nodes.ShouldContain("$.Items[0]|5|5|Success|");
    }

    class Faulty
    {
        private readonly bool _fail;

        public Faulty(bool fail)
        {
            _fail = fail;
        }

        public string Value => _fail ? throw new InvalidOperationException("failure") : "text";
    }

    [Test]
    public void It_should_capture_exceptions()
    {
        var expected = new { A = new Faulty(false), B = new Faulty(true), C = new Faulty(true) };
        var actual = new { A = new Faulty(true), B = new Faulty(false), C = new Faulty(true) };

        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);
        var nodes = tree.Details.Root.EnumerateAll()
            .Select(n => $"{n.Path}|{n.Expectation}|{n.Value}|{n.VerificationStatus}")
            .ToArray();

        nodes.ShouldBe(new[]
        {
            "$|<object>|<object>|Success", "$.A|<object>|<object>|Success",
            "$.A.Value|text|InvalidOperationException: failure|Exception",
            "$.B|<object>|<object>|Success",
            "$.B.Value|InvalidOperationException: failure|text|Exception",
            "$.C|<object>|<object>|Success",
            "$.C.Value|InvalidOperationException: failure|InvalidOperationException: failure|Exception"
        });

        tree.Details.VerificationStatus.ShouldBe(ParameterVerificationStatus.Exception);
        tree.Details.VerificationMessage.ShouldContain("$.A.Value: System.InvalidOperationException : failure");
        tree.Details.VerificationMessage.ShouldContain("$.B.Value: System.InvalidOperationException : failure");
        tree.Details.VerificationMessage.ShouldContain("$.C.Value: System.InvalidOperationException : failure");
    }

    private void AssertNodes(IEnumerable<ITreeParameterNodeResult> nodes, params string[] expected)
    {
        var actual = DumpNodes(nodes);
        actual.ShouldBe(expected);
    }

    private static string[] DumpNodes(IEnumerable<ITreeParameterNodeResult> nodes)
    {
        return nodes.Select(n => $"{n.Path}|{n.Expectation}|{n.Value}|{n.VerificationStatus}|{n.VerificationMessage}").ToArray();
    }

    class Node
    {
        public Node Parent { get; set; }
        public string Name { get; set; }
        public List<Node> Children { get; } = new();
    }
}