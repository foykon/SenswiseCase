using Application.Contracts.Users;
using Application.Queries.Users;
using Application.Repositories;
using MapsterMapper;
using MediatR;
using System;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Users
{
    public sealed class ListUsersHandler : IRequestHandler<ListUsersQuery, PagedResult<UserResponse>>
    {
        private readonly IUserRepository _users;
        private readonly IMapper _mapper;

        public ListUsersHandler(IUserRepository users, IMapper mapper)
        {
            _users = users;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserResponse>> Handle(ListUsersQuery req, CancellationToken ct)
        {
            var q = _users.Query();

            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.Trim().ToLower();
                q = q.Where(x =>
                    x.FirstName.ToLower().Contains(s) ||
                    x.LastName.ToLower().Contains(s));
            }

            var total = await q.CountAsync(ct);

            var items = await q
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ProjectToType<UserResponse>() // server-side projection
                .ToListAsync(ct);

            return new PagedResult<UserResponse>
            {
                Items = items,
                Page = req.Page,
                PageSize = req.PageSize,
                TotalCount = total
            };
        }
    }
}
