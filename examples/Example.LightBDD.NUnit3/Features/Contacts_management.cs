using LightBDD.Example.AcceptanceTests.NUnit3.Features.Contexts;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.NUnit3;

namespace LightBDD.Example.AcceptanceTests.NUnit3.Features
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
            Runner.WithContext<ContactsManagementContext>().RunScenario(
                _ => _.Given_my_contact_book_is_empty(),
                _ => _.When_I_add_new_contacts(),
                _ => _.Then_all_contacts_should_be_available_in_the_contact_book());
        }

        [Scenario]
        [Label("Ticket-9")]
        public void Contact_book_should_allow_me_to_remove_contacts()
        {
            Runner.WithContext<ContactsManagementContext>().RunScenario(
                _ => _.Given_my_contact_book_is_filled_with_contacts(),
                _ => _.When_I_remove_one_contact(),
                _ => _.Then_the_contact_book_should_not_contain_removed_contact_any_more(),
                _ => _.Then_the_contact_book_should_contains_all_other_contacts());
        }

        [Scenario]
        [Label("Ticket-9")]
        public void Contact_book_should_allow_me_to_remove_all_contacts()
        {
            Runner.WithContext<ContactsManagementContext>().RunScenario(
                _ => _.Given_my_contact_book_is_filled_with_many_contacts(),
                _ => _.When_I_clear_it(),
                _ => _.Then_the_contact_book_should_be_empty());
        }
    }
}