using System;
using System.Linq;
using LightBDD.XUnit.UnitTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.XUnit.UnitTests
{
    public class ScenarioTest
    {
        private readonly ITestOutputHelper _output;

        public ScenarioTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void It_should_support_passing_test()
        {
            string code = @"
using LightBDD;

public class TestClass
{
    [Scenario]
    public void Passing () { }
}
";
            ExecuteTests(code, AssertMessage<ITestPassed>);
        }

        [Fact]
        public void It_should_support_failing_test()
        {
            string code = @"
using LightBDD;
using System;

public class TestClass
{
    [Scenario]
    public void Failing () { throw new Exception(); }
}
";
            ExecuteTests(code, AssertMessage<ITestFailed>);
        }

        [Fact]
        public void It_should_support_skipped_test_at_runtime()
        {
            string code = @"
using LightBDD;
using System;

public class TestClass
{
    [Scenario]
    public void Ignored () { ScenarioAssert.Ignore(""reason""); }
}
";
            ExecuteTests(code, AssertMessage<ITestSkipped>);
        }

        [Fact]
        public void It_should_support_passing_parameterized_test()
        {
            string code = @"
using LightBDD;
using Xunit;

public class TestClass
{
    [Scenario]
    [InlineData(0)]
    [InlineData(1)]
    public void Passing (int arg) { }
}
";
            ExecuteTests(code, sink => AssertMessage<ITestPassed>(sink, 2));
        }

        [Fact]
        public void It_should_support_failing_parameterized_test()
        {
            string code = @"
using LightBDD;
using Xunit;

public class TestClass
{
    [Scenario]
    [InlineData(0)]
    [InlineData(1)]
    public void IsZero (int arg) { Assert.Equal(0, arg); }
}
";
            ExecuteTests(code, sink => { AssertMessage<ITestPassed>(sink); AssertMessage<ITestFailed>(sink); });
        }

        [Fact]
        public void It_should_support_ignored_parameterized_test()
        {
            string code = @"
using LightBDD;
using Xunit;

public class TestClass
{
    [Scenario]
    [InlineData(0)]
    [InlineData(1)]
    public void IsZero (int arg) { if(arg!=0) ScenarioAssert.Ignore(""reason""); }
}
";
            ExecuteTests(code, sink => { AssertMessage<ITestPassed>(sink); AssertMessage<ITestSkipped>(sink); });
        }

        [Fact]
        public void It_should_support_declaratively_skipped_test()
        {
            string code = @"
using LightBDD;
using Xunit;

public class TestClass
{
    [Scenario(Skip=""abc"")]
    public void Skipped () { }
}
";
            ExecuteTests(code, AssertMessage<ITestSkipped>);
        }

        [Fact]
        public void It_should_support_generic_parameterized_test()
        {
            string code = @"
using LightBDD;
using Xunit;

public class TestClass
{
    [Scenario]
    [InlineData(1)]
    [InlineData(true)]
    [InlineData(2d)]
    [InlineData('c')]
    public void Generic<T> (T arg) { }
}
";
            ExecuteTests(code, sink => AssertMessage<ITestPassed>(sink, 4));
        }

        [Fact]
        public void It_should_fail_if_parameterized_test_has_wrong_arguments()
        {
            string code = @"
using LightBDD;
using Xunit;

public class TestClass
{
    [Scenario]
    [MemberData(""InvalidMember"")]
    public void TestMethod (int arg) { }
}
";
            ExecuteTests(code, AssertMessage<ITestFailed>);
        }

        private static void ExecuteTests(string code, Action<MessageSink> assertions)
        {
            var assemblyName = TestAssemblyCompiler.Compile(code);
            var sink = new MessageSink();
            using (var xunit2 = new Xunit2(new NullSourceInformationProvider(), assemblyName))
            {
                xunit2.RunAll(sink, TestFrameworkOptions.ForDiscovery(), TestFrameworkOptions.ForExecution());
                sink.WaitTillFinished();
                assertions(sink);
            }
        }

        private void AssertMessage<T>(MessageSink sink) where T : ITestCaseMessage
        {
            AssertMessage<T>(sink, 1);
        }

        private void AssertMessage<T>(MessageSink sink, int expectedCount) where T : ITestCaseMessage
        {
            var messages = sink.GetMessages<T>();
            foreach (var message in messages)
                _output.WriteLine(message.TestCase.DisplayName);
            Assert.True(messages.Length == expectedCount, string.Format(" Expected to receive {0} {1} message(s), got {2}", expectedCount, typeof(T).Name, messages.Length));
        }
    }
}
