using Example.Domain.Domain;
using LightBDD.Fixie3;
using LightBDD.Framework;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;

namespace Example.LightBDD.Fixie3.Features
{
    [Label("Story-11")]
    [FeatureDescription(
@"In order to manage users
As an admin
I want to be able to retrieve user data")]
    public partial class User_management_feature
    {
        [Scenario]
        public void Retrieving_user_details()
        {
            Runner.RunScenario(
                _ => Given_a_user_with_id_name_surname_and_email(124, "Joe", "Johnson", "jj@gmail.com"),
                _ => When_I_request_user_details_for_id(124),
                _ => Then_I_should_receive_user_with_id_name_surname_and_email(124, "Joe", "Johnson", "jj@gmail.com")
            );
        }

        [Scenario]
        public void Validating_user_details()
        {
            Runner.RunScenario(
                _ => Given_a_user_with_id_name_surname_and_email(124, "Joe", "Johnson", "jj@gmail.com"),
                _ => When_I_request_user_details_for_id(124),
                _ => Then_I_should_receive_user_with_id_name_surname_and_email(124,
                    Expect.To.BeLikeIgnoreCase("Joe"),
                    Expect.To.Not.BeEmpty<string>(),
                    Expect.To.MatchIgnoreCase("[a-z]+@([a-z]+)(\\.[a-z]+)+"))
            );
        }

        [Scenario]
        public void User_search()
        {
            Runner.RunScenario(

                _ => Given_users(Table.For(
                    new User(1, "Joe", "Andersen", "jj@foo.com"),
                    new User(2, "Henry", "Hansen", "henry123@foo.com"),
                    new User(3, "Marry", "Davis", "ma31@bar.com"),
                    new User(4, "Monica", "Larsen", "monsmi22@bar.com"))),

                _ => When_I_search_for_users_by_surname_pattern(".*sen"),

                _ => Then_I_should_receive_users(Table.ExpectData(
                    new User(1, "Josh", "Andersen", "jj@foo.com"),
                    new User(2, "Henry", "Hansen", "henry123@foo.com"))));
        }

        [Scenario]
        public void Validating_found_users()
        {
            Runner.RunScenario(

                _ => Given_users(Table.For(
                    new User(0, "Joe", "Andersen", "jj@foo.com"),
                    new User(2, "Henry", "Hansen", "henry123@foo2.com"),
                    new User(3, "Marry", "Davis", "ma31@bar.com"),
                    new User(4, "Monica", "Larsen", "monsmi22@bar.com"))),

                _ => When_I_search_for_users_by_surname_pattern(".*sen"),

                _ => Then_I_should_receive_users(Table.Validate<User>(builder => builder
                    .WithColumn(x => x.Id, Expect.To.BeGreaterThan(0))
                    .WithColumn(x => x.Name, Expect.To.Not.BeEmpty())
                    .WithColumn(x => x.Surname, Expect.To.BeLikeIgnoreCase("*sen"))
                    .WithColumn(x => x.Email, Expect.To.MatchIgnoreCase("[\\w]+@([a-z]+)(\\.[a-z]+)+")))));
        }
    }
}