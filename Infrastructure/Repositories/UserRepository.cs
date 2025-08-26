using Application.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public sealed class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext ctx) : base(ctx) { }

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            var vo = Email.Create(email);
            return _set.AsNoTracking().AnyAsync(u => u.Email == vo, ct);
        }
    }
}
