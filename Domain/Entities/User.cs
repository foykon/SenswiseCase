using Domain.Abstractions;
using Domain.Common;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : BaseEntity, IHasTimestamps
    {
        public const int FirstNameMaxLen = 100;
        public const int LastNameMaxLen = 100;
        public const int AddressMaxLen = 500;

        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public Email Email { get; private set; } = default!;
        public string? Address { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private User() { }

        private User(string firstName, string lastName, Email email, string? address)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), FirstNameMaxLen);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), LastNameMaxLen);
            Email = email;
            Address = Guard.TrimToNull(address, AddressMaxLen);

            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public static User Create(string firstName, string lastName, string email, string? address)
            => new User(firstName, lastName, Email.Create(email), address);

        public void Update(string firstName, string lastName, string email, string? address)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), FirstNameMaxLen);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), LastNameMaxLen);
            Email = Email.Create(email);
            Address = Guard.TrimToNull(address, AddressMaxLen);
            Touch();
        }

        public void ChangeEmail(string email)
        {
            Email = Email.Create(email);
            Touch();
        }

        public void ChangeAddress(string? address)
        {
            Address = Guard.TrimToNull(address, AddressMaxLen);
            Touch();
        }

        private void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
