using System;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests
{
    [TestFixture]
    public class State_tests
    {
        [Test]
        public void State_should_be_uninitialized_by_default()
        {
            State<int> state = default;
            Assert.False(state.IsInitialized);
            Assert.False(new State<bool>().IsInitialized);
        }

        [Test]
        public void It_should_be_possible_to_set_and_retrieve_state()
        {
            var state = new State<int>();
            Assert.False(state.IsInitialized);

            state = 0;
            AssertInitializedValue(state, 0);

            state = new State<int>(5);
            AssertInitializedValue(state, 5);

            int x = state;
            Assert.That(x, Is.EqualTo(5));

            var state2 = new State<string>("text");
            AssertInitializedValue(state2, "text");

            state2 = null;
            AssertInitializedValue(state2, null);

            string s = state2;
            Assert.That(s, Is.Null);
        }

        [Test]
        public void GetValue_should_throw_if_state_is_not_initialized()
        {
            State<bool> state = default;

            var ex = Assert.Throws<InvalidOperationException>(() => state.GetValue());
            Assert.That(ex.Message, Is.EqualTo("The Boolean state is not initialized."));

            ex = Assert.Throws<InvalidOperationException>(() =>
            {
                bool x = state;
            });
            Assert.That(ex.Message, Is.EqualTo("The Boolean state is not initialized."));

            ex = Assert.Throws<InvalidOperationException>(() => state.GetValue("myField"));
            Assert.That(ex.Message, Is.EqualTo("The myField state is not initialized."));
        }

        [Test]
        public void GetValueOrDefault_should_return_value_if_initialized_or_default_if_not_or_value_is_null()
        {
            var defaultInt = 5;
            var defaultString = "default";

            State<int> state = default;
            Assert.AreEqual(defaultInt, state.GetValueOrDefault(defaultInt));
            state = 0;
            Assert.AreEqual(0, state.GetValueOrDefault(defaultInt));

            State<string> state2 = default;
            Assert.AreEqual(defaultString, state2.GetValueOrDefault(defaultString));

            state2 = "value";
            Assert.AreEqual("value", state2.GetValueOrDefault(defaultString));

            state2 = null;
            Assert.AreEqual(defaultString, state2.GetValueOrDefault(defaultString));
        }

        private static void AssertInitializedValue<T>(State<T> state, T expected)
        {
            Assert.True(state.IsInitialized);
            Assert.AreEqual(expected, state.GetValue());
        }
    }
}
