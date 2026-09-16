using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities
{
    public class Customer : Entity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }

        public Customer(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (!email.Contains('@') || !email.Contains('.'))
                throw new ArgumentException("Invalid email.", nameof(email));

            Name = name;
            Email = email;
        }
    }
}
