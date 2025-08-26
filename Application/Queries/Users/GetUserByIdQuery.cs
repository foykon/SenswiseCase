using Application.Contracts.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Users
{
    public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;

}
