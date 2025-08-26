using Application.Contracts.Users;
using Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public static class MapsterConfig
    {
        public static void Register()
        {
            // Entity -> DTO
            TypeAdapterConfig<User, UserResponse>
                .NewConfig()
                .Map(d => d.Email, s => s.Email.Value);
        }
    }
}
