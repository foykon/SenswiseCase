using Application.Contracts.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Users
{
    public sealed record ListUsersQuery(int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<PagedResult<UserResponse>>;
}
