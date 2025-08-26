using Infrastructure.Db.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Db;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Postgres connection string is missing.");

            services.AddSingleton<AuditingInterceptor>();

            services.AddDbContext<AppDbContext>((sp, opt) =>
            {
                opt.UseNpgsql(connectionString);
                opt.UseSnakeCaseNamingConvention();
                opt.AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
            });

            services.AddScoped<Application.Repositories.IUserRepository, Infrastructure.Repositories.UserRepository>();
            services.AddScoped<Application.Repositories.IUnitOfWork, Infrastructure.Repositories.UnitOfWork>();


            return services;
        }
    }
}
