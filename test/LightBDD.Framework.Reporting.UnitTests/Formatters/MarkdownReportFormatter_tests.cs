using System.IO;
using System.Text;
using LightBDD.Core.Results;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.Reporting.UnitTests.Formatters;

[TestFixture]
public class MarkdownReportFormatter_tests
{
    private IReportFormatter _subject;

    #region Setup/Teardown

    [SetUp]
    public void SetUp()
    {
        _subject = new MarkdownReportFormatter();
    }

    #endregion

    [Test]
    public void Should_format_feature_with_description()
    {
        var result = ReportFormatterTestData.GetFeatureResultWithDescription();
        var text = FormatResults(result);
        TestContext.WriteLine(text);
        const string expectedText = @"# Summary

| Entry              | Value |
|              ----: | :---- |
| Execution Start    | 2014-09-23 19:21:58 UTC |
| Execution Duration | 1m 02s |
| **Overall Status** | :red_circle: Failed |
| Total Features     | 1 |
| Total Scenarios    | 2 |
| Failed Scenarios   | 1 |
| Ignored Scenarios  | 1 |
| Total Steps        | 10 |
| Passed Steps       | 3 |
| Bypassed Steps     | 1 |
| Failed Steps       | 2 |
| Ignored Steps      | 2 |
| Not Run Steps      | 2 |

# Features


## My feature :label:`Label 1`
> My feature
> long description


### :warning: Scenario: name :label:`Label 2` :file_folder:`categoryA` :watch:`1m 02s`
> My scenario
> long description

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: call step1 ""`arg1`"" :watch:`1m 01s`
#### &nbsp;&nbsp;&nbsp; :warning: Step 2: step2 :watch:`1s 100ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 2.1: substep 1 :watch:`100ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 2.2: substep 2 :watch:`1s`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :warning: Step 2.3: substep 3 :watch:`0ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :red_circle: Step 2.3.1: sub-substep 1
**$table1**:
<div style=""overflow-x: auto;"">

|#|Key   |X             |Y             |
|-|------|--------------|--------------|
|☑|`Key1`|`1`           |`2`           |
|❗|`Key2`|`1` / `2`     |`4`           |
|➖|`Key3`|`<none>` / `3`|`<none>` / `6`|
|➕|`Key4`|`3` / `<none>`|`6` / `<none>`|

</div>

**$table2**:
<div style=""overflow-x: auto;"">

|Key   |X  |Y  |
|------|---|---|
|`Key1`|`1`|`2`|
|`Key2`|`2`|`4`|
|`Key3`|`3`|`6`|

</div>

#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_circle: Step 2.3.2: sub-substep 2
> [!IMPORTANT]
> <pre>
> Step 2: Not implemented yet
> </pre>

> [!NOTE]
> <pre>
> Step 1: multiline
> comment
> Step 1: comment 2
> Step 2.3: sub-comment
> Step 2.3.1: sub-sub-multiline
> comment
> </pre>

> [!NOTE]
> Step 2.3.1: [:link: attachment1](file1.png)

---


### :red_circle: Scenario: name2 ""arg1"" :file_folder:`categoryB``categoryC` :watch:`2s 157ms`
#### &nbsp;&nbsp;&nbsp; :large_blue_diamond: Step 1: step3 :watch:`2s 107ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 2: step4 :watch:`50ms`
#### &nbsp;&nbsp;&nbsp; :white_circle: Step 3: step5
> [!IMPORTANT]
> <pre>
> Step 1: bypass reason
> Step 2: Expected: True
> 	  But was: False
> </pre>

---
";
        Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
    }

    [Test]
    public void Should_format_feature_without_description_nor_label_nor_details()
    {
        var result = ReportFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();
        var text = FormatResults(result);
        TestContext.WriteLine(text);
        const string expectedText = @"# Summary

| Entry              | Value |
|              ----: | :---- |
| Execution Start    | 2014-09-23 19:21:58 UTC |
| Execution Duration | 25ms |
| **Overall Status** | :white_check_mark: Passed |
| Total Features     | 1 |
| Total Scenarios    | 1 |
| Ignored Scenarios  | 1 |
| Total Steps        | 2 |
| Passed Steps       | 1 |
| Ignored Steps      | 1 |

# Features


## My feature


### :warning: Scenario: name :watch:`25ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: step1 :watch:`20ms`
#### &nbsp;&nbsp;&nbsp; :warning: Step 2: step2 :watch:`5ms`
---
";
        Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
    }

    [Test]
    public void Should_format_multiple_features()
    {
        var results = ReportFormatterTestData.GetMultipleFeatureResults();

        var text = FormatResults(results);
        TestContext.WriteLine(text);
        const string expectedText = @"# Summary

| Entry              | Value |
|              ----: | :---- |
| Execution Start    | 2014-09-23 19:21:58 UTC |
| Execution Duration | 3s 020ms |
| **Overall Status** | :white_check_mark: Passed |
| Total Features     | 2 |
| Total Scenarios    | 2 |
| Passed Scenarios   | 2 |
| Total Steps        | 2 |
| Passed Steps       | 2 |

# Features


## My feature


### :white_check_mark: Scenario: scenario1 :file_folder:`categoryA` :watch:`20ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: step1 :watch:`20ms`
---


## My feature2


### :white_check_mark: Scenario: scenario1 :file_folder:`categoryB` :watch:`20ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: step1 :watch:`20ms`
---
";
        Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
    }

    [Test]
    public void Should_format_scenarios_in_order()
    {
        var results = ReportFormatterTestData.GetFeatureWithUnsortedScenarios();

        var text = FormatResults(results);
        TestContext.WriteLine(text);
        const string expectedText = @"# Summary

| Entry              | Value |
|              ----: | :---- |
| Execution Start    | 2014-09-23 19:21:57 UTC |
| Execution Duration | 5s |
| **Overall Status** | :white_check_mark: Passed |
| Total Features     | 1 |
| Total Scenarios    | 3 |
| Passed Scenarios   | 3 |
| Total Steps        | 3 |
| Passed Steps       | 3 |

# Features


## My Feature


### :white_check_mark: Scenario: scenario A :label:`lab B` :watch:`2s`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: step
---


### :white_check_mark: Scenario: scenario B :label:`lab C` :watch:`5s`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: step
---


### :white_check_mark: Scenario: scenario C :label:`lab A` :watch:`2s`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: step
---
";
        Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
    }

    [Test]
    public void Should_format_verifiable_trees()
    {
        var expected = new
        {
            Name = "John",
            Surname = "Johnson",
            Address = new { Street = "High Street", PostCode = "AB1 7BA", City = "London", Country = "UK" },
            Records = new[] { "AB-1", "AB-2", "AB-34", "spe`cial" }
        };
        var actual = new
        {
            Name = "Johnny",
            Surname = "Johnson",
            Address = new { Street = "High Street", PostCode = "AB1 7BC", City = "London", Country = "UK" },
            Records = new[] { "AB-1", "AB-2", "AB-3", "spe`cial", "AB-4" }
        };

        var tree = Tree.ExpectEquivalent(expected);
        tree.SetActual(actual);

        var results = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
        var text = FormatResults(results);
        TestContext.WriteLine(text);
        const string expectedText = @"# Summary

| Entry              | Value |
|              ----: | :---- |
| Execution Start    | 2014-09-23 19:21:57 UTC |
| Execution Duration | 2s |
| **Overall Status** | :red_circle: Failed |
| Total Features     | 1 |
| Total Scenarios    | 1 |
| Failed Scenarios   | 1 |
| Total Steps        | 1 |
| Failed Steps       | 1 |

# Features


## My Feature


### :red_circle: Scenario: scenario A :label:`lab B` :watch:`2s`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 1: step
**$tree**:
<div style=""overflow-x: auto;"">

| # | Node | Value |
|---| ---- | ----- |
| ☑ | `$` | `<object>` |
| ☑ | `$.Address` | `<object>` |
| ☑ | `$.Address.City` | `London` |
| ☑ | `$.Address.Country` | `UK` |
| ❗ | `$.Address.PostCode` | `AB1 7BA` / `AB1 7BC` |
| ☑ | `$.Address.Street` | `High Street` |
| ❗ | `$.Name` | `John` / `Johnny` |
| ❗ | `$.Records` | `<array:4>` / `<array:5>` |
| ☑ | `$.Records[0]` | `AB-1` |
| ☑ | `$.Records[1]` | `AB-2` |
| ❗ | `$.Records[2]` | `AB-34` / `AB-3` |
| ☑ | `$.Records[3]` | ``spe`cial`` |
| ❗ | `$.Records[4]` | `<none>` / `AB-4` |
| ☑ | `$.Surname` | `Johnson` |

</div>

---
";
        Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
    }

    private string FormatResults(params IFeatureResult[] results)
    {
        using (var memory = new MemoryStream())
        {
            _subject.Format(memory, results);
            return Encoding.UTF8.GetString(memory.ToArray());
        }
    }
}