namespace LightBDD.Example.AcceptanceTests.XUnit.Features
{
    [FeatureDescription(
@"In order to maintain my contact book
As an application user
I want to add, browse and remove my contacts")]
    [Label("Story-6")]
    public partial class Contacts_management
    {
        [Scenario]
        [Label("Ticket-8")]
        public void Contact_book_should_allow_me_to_add_multiple_contacts()
        {
            Runner.RunScenario<ScenarioContext>(
                Given_my_contact_book_is_empty,
                When_I_add_new_contacts,
                Then_all_contacts_should_be_available_in_the_contact_book);
        }

        [Scenario]
        [Label("Ticket-9")]
        public void Contact_book_should_allow_me_to_remove_contacts()
        {
            Runner.RunScenario<ScenarioContext>(
                Given_my_contact_book_is_filled_with_contacts,
                When_I_remove_one_contact,
                Then_the_contact_book_should_not_contain_removed_contact_any_more,
                Then_the_contact_book_should_contains_all_other_contacts);
        }

        [Scenario]
        [Label("Ticket-9")]
        public void Contact_book_should_allow_me_to_remove_all_contacts()
        {
            Runner.RunScenario<ScenarioContext>(
                Given_my_contact_book_is_filled_with_many_contacts,
                When_I_clear_it,
                Then_the_contact_book_should_be_empty);
        }
    }
}