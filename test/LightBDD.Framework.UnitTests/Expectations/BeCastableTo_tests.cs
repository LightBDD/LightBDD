using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Expectations;

[TestFixture]
public class BeCastableTo_tests : ExpectationTests
{
    protected override IEnumerable<IExpectationScenario> GetScenarios()
    {
        yield return new ExpectationScenario<object>(
                "be castable to 'String'",
                x => x.BeCastableTo<string>())
            .WithMatchingValues("abc", "", null)
            .WithNotMatchingValue('c', "expected: be castable to 'String', but got 'Char' type");

        yield return new ExpectationScenario<object>(
                "be castable to 'Int32'",
                x => x.BeCastableTo<int>())
            .WithMatchingValues(5, 5L, (byte)5, 5f, 5d, 5m, (int?)5)
            .WithNotMatchingValue("", "expected: be castable to 'Int32', but got 'String' type");
    }

    [Test]
    public void Casting_from_null_to_value_type_should_fail()
    {
        var result = Expect.To.BeCastableTo<int>().Verify(null, ValueFormattingServices.Current);
        Assert.False(result);
        Assert.That(result.Message, Is.EqualTo("expected: be castable to 'Int32', but got '<null>' type"));
    }

    [Test]
    public void Casting_of_numerics_with_overflow_should_fail()
    {
        var result = Expect.To.BeCastableTo<int>().Verify(long.MaxValue, ValueFormattingServices.Current);
        Assert.False(result);
        result.Message.ShouldBe("expected: be castable to 'Int32', but got 'Int64' type with value '9223372036854775807' which cannot be cast: Value was either too large or too small for an Int32.");
    }

    [Test]
    public void Casting_of_numerics_with_precision_loss_should_fail()
    {
        var result = Expect.To.BeCastableTo<int>().Verify(5.14, ValueFormattingServices.Current);
        Assert.False(result);
        result.Message.ShouldBe("expected: be castable to 'Int32', but got 'Double' type with value '5.14' which cannot be cast without precision loss");
    }

    [Test]
    public void Casting_null_to_Nullable()
    {
        var result = Expect.To.BeCastableTo<int?>().Verify(null, ValueFormattingServices.Current);
        Assert.True(result);
    }
}