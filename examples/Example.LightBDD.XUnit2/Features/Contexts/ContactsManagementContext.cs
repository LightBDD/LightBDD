using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Expectations;
using Xunit;

namespace Example.LightBDD.XUnit2.Features.Contexts
{
    public class ContactsManagementContext
    {
        private ContactBook ContactBook = new ContactBook();
        private List<Contact> AddedContacts = new List<Contact>();
        private List<Contact> RemovedContacts = new List<Contact>();
        private Contact[] SearchResults = new Contact[0];

        public void Given_my_contact_book_is_empty()
        {
            ContactBook = new ContactBook();
        }

        public void When_I_add_new_contacts()
        {
            AddSomeContacts();
        }

        public void Then_all_contacts_should_be_available_in_the_contact_book()
        {
            Assert.Equal(
                   AddedContacts,
                   ContactBook.Contacts.ToArray());
        }

        public void Given_my_contact_book_is_filled_with_contacts()
        {
            ContactBook = new ContactBook();
            AddSomeContacts();
        }

        public void When_I_remove_one_contact()
        {
            RemoveContact(ContactBook.Contacts.First());
        }

        public void Then_the_contact_book_should_not_contain_removed_contact_any_more()
        {
            Assert.Equal(
                 Enumerable.Empty<Contact>(),
                 ContactBook.Contacts.Where(c => RemovedContacts.Contains(c)).ToArray());
        }

        public void Then_the_contact_book_should_contains_all_other_contacts()
        {
            Assert.Equal(
                   ContactBook.Contacts.ToArray(),
                   AddedContacts.Except(RemovedContacts).ToArray());
        }

        public void Given_my_contact_book_is_filled_with_many_contacts()
        {
            for (var i = 0; i < 10000; ++i)
                ContactBook.AddContact(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture));
        }

        public void When_I_clear_it()
        {
            foreach (var contact in ContactBook.Contacts.ToArray())
                RemoveContact(contact);
            StepExecution.Current.Bypass("Contact book clearing is not implemented yet. Contacts are removed one by one.");
        }

        private void RemoveContact(Contact contact)
        {
            RemovedContacts.Add(contact);
            ContactBook.Remove(contact.Name);
        }

        public void Then_the_contact_book_should_be_empty()
        {
            Assert.Empty(ContactBook.Contacts);
        }

        private void AddSomeContacts()
        {
            var contacts = new[]
             {
                new Contact("Jack", "123-456-789","justjack@hotmail.com"),
                new Contact("Samantha", "321-654-987","samantha359@gmai.com"),
                new Contact("Josh", "132-465-798","jos4@gmail.com")
            };

            foreach (var contact in contacts)
                AddContact(contact);
        }

        private void AddContact(Contact contact)
        {
            AddedContacts.Add(contact);
            ContactBook.AddContact(contact.Name, contact.PhoneNumber, contact.Email);
        }

        public void Given_I_added_contact_with_name_phone_and_email(string name, string phone, string email)
        {
            AddContact(new Contact(name, phone, email));
        }

        public void When_I_search_for_contacts_by_phone_starting_with(string with)
        {
            SearchResults = ContactBook.SearchByPhoneStartingWith(with).ToArray();
        }

        public void Then_the_result_should_contain_name_with_phone_and_email(string name, Expected<string> phone, Expected<string> email)
        {
            var contact = SearchResults.First(x => x.Name == name);
            phone.SetActual(contact.PhoneNumber);
            email.SetActual(contact.Email);
        }
    }
}