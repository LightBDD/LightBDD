namespace Example.Domain.Domain
{
    public class User
    {
        public User(int id, string name, string surname, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
        }

        public int Id { get; }
        public string Name { get; }
        public string Surname { get; }
        public string Email { get; }
    }
}