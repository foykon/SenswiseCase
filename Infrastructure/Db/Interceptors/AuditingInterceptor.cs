using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Db.Interceptors
{
    public sealed class AuditingInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var ctx = eventData.Context;
            if (ctx is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            var now = DateTime.UtcNow;

            foreach (var entry in ctx.ChangeTracker.Entries<IHasTimestamps>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(IHasTimestamps.CreatedAt)).CurrentValue = now;
                    entry.Property(nameof(IHasTimestamps.UpdatedAt)).CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(IHasTimestamps.UpdatedAt)).CurrentValue = now;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
