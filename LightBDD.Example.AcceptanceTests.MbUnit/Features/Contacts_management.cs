using MbUnit.Framework;

namespace LightBDD.Example.AcceptanceTests.MbUnit.Features
{
    [FeatureDescription(
@"In order to maintain my contact book
As an application user
I want to add, browse and remove my contacts")]
    [Label("Story-6")]
    [TestFixture]
    [Parallelizable(TestScope.All)]
    public partial class Contacts_management
    {
        [Test]
        [Label("Ticket-8")]
        public void Contact_book_should_allow_me_to_add_multiple_contacts()
        {
            Runner.RunScenario<ScenarioContext>(
                Given_my_contact_book_is_empty,
                When_I_add_new_contacts,
                Then_all_of_expected_contacts_would_be_available_in_contact_book);
        }

        [Test]
        [Label("Ticket-9")]
        public void Contact_book_should_allow_me_to_remove_contacts()
        {
            Runner.RunScenario<ScenarioContext>(
                Given_my_contact_book_is_filled_with_contacts,
                When_I_remove_contact,
                Then_contact_book_does_not_contain_removed_contact_any_more,
                Then_contact_book_still_contains_other_contacts);
        }

        [Test]
        [Label("Ticket-9")]
        public void Contact_book_should_allow_me_to_remove_all_contacts()
        {
            Runner.RunScenario<ScenarioContext>(
                Given_my_contact_book_is_filled_with_many_contacts,
                When_I_clear_it,
                Then_contact_book_is_empty);
        }
    }
}