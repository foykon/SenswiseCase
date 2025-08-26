using Application.Contracts.Users;
using Application.Queries.Users;
using Application.Repositories;
using MediatR;
using Mapster;  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Users
{
    public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResponse?>
    {
        private readonly IUserRepository _users;

        public GetUserByIdHandler(IUserRepository users) => _users = users;

        public async Task<UserResponse?> Handle(GetUserByIdQuery req, CancellationToken ct)
        {
            var e = await _users.GetByIdAsync(req.Id, ct);
            return e is null ? null : e.Adapt<UserResponse>();
        }
    }
}
