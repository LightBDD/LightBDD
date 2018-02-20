using Example.LightBDD.XUnit2.Features.Contexts;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;

namespace Example.LightBDD.XUnit2.Features
{
    /// <summary>
    /// This feature class presents the usage of contextual scenarios that are used in conjunction with extended step format.
    /// With contextual scenarios it is possible to share state between all steps, which is useful in the situation where
    /// sharing state via feature class fields is not desired.
    /// 
    /// To use contextual scenarios, the Runner.WithContext() method has to be used and the context object can be then accessed by step with lambda parameter.
    /// The scenario below shows the recommended usage of context, where given-when-then steps are declared on the context class (it is however not enforced rule).
    /// 
    /// Another feature presented here is that if lambda parameter name is 1 character only, it is ignored in the reports, and the step type (GIVEN/WHEN/THEN/etc)
    /// is inferred from the step method name.
    /// 
    /// More information on contextual scenarios can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#contextual-scenarios
    /// </summary>
    [FeatureDescription(
@"In order to maintain my contact book
As an application user
I want to add, browse and remove my contacts")]
    [Label("Story-6")]
    public class Contacts_management : FeatureFixture
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
                c => c.Given_my_contact_book_is_filled_with_many_contacts(),
                c => c.When_I_clear_it(),
                c => c.Then_the_contact_book_should_be_empty());
        }
    }
}