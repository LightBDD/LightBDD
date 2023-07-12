using Example.Domain.Domain;
using LightBDD.Framework.Parameters;
using LightBDD.XUnit2;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class User_management_feature : FeatureFixture
    {
        private List<User> _users = new List<User>();
        private User[] _selected = Array.Empty<User>();

        private void Given_a_user_with_id_name_surname_and_email(int id, string name, string surname, string email)
        {
            _users.Add(new User(id, name.ToUpperInvariant(), surname.ToUpperInvariant(), email));
        }

        private void When_I_request_user_details_for_id(int id)
        {
            _selected = _users.Where(x => x.Id == id).ToArray();
        }

        private void Then_I_should_receive_user_with_id_name_surname_and_email(Verifiable<int> id, Verifiable<string> name, Verifiable<string> surname, Verifiable<string> email)
        {
            var user = _selected.FirstOrDefault();
            id.SetActual(user?.Id ?? 0);
            name.SetActual(user?.Name);
            surname.SetActual(user?.Surname);
            email.SetActual(user?.Email);
        }

        private void Given_users(InputTable<User> users)
        {
            _users = users.ToList();
        }

        private void When_I_search_for_users_by_surname_pattern(string pattern)
        {
            _selected = _users.Where(x => Regex.IsMatch(x.Surname, pattern)).ToArray();
        }

        void Then_I_should_receive_users(VerifiableDataTable<User> users)
        {
            users.SetActual(_selected);
        }

        private void Then_I_should_receive_users(TableValidator<User> users)
        {
            users.SetActual(_selected);
        }
    }
}