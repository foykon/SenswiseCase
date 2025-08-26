using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Db.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> e)
        {
            // to stiiring
            var emailConv = new ValueConverter<Email, string>(
                v => v.Value,
                v => Email.Create(v)
            );

            e.ToTable("users");
            e.HasKey(x => x.Id);

            e.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(User.FirstNameMaxLen);

            e.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(User.LastNameMaxLen);

            e.Property(x => x.Email)
                .IsRequired()
                .HasConversion(emailConv)
                .HasMaxLength(200)
                .HasColumnType("citext");

            e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.Address)
                .HasMaxLength(User.AddressMaxLen);

            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();

            e.UseXminAsConcurrencyToken();
        }
    }
}
