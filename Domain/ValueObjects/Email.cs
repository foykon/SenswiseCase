using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        public string Value { get; }

        private Email(string value) => Value = value;

        public static Email Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new DomainException("Email is required.");

            var trimmed = input.Trim();

            try
            {
                var addr = new MailAddress(trimmed);
                return new Email(addr.Address); // normalize
            }
            catch
            {
                throw new DomainException("Email is invalid.");
            }
        }

        public bool Equals(Email? other) =>
            other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object? obj) => obj is Email e && Equals(e);

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

        public override string ToString() => Value;
    }
}
