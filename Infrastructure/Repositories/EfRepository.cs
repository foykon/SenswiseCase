using Application.Repositories;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _ctx;
        protected readonly DbSet<T> _set;

        public EfRepository(AppDbContext ctx)
        {
            _ctx = ctx;
            _set = ctx.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _set.FindAsync(new object?[] { id }, ct);

        public virtual Task AddAsync(T entity, CancellationToken ct = default)
        {
            _set.Add(entity);
            return Task.CompletedTask;
        }

        public virtual void Remove(T entity) => _set.Remove(entity);

        public virtual IQueryable<T> Query() => _set.AsQueryable();
    }
}
