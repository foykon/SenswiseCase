using Application.Commands.Users;
using Application.Contracts.Users;
using Application.Repositories;
using Domain.Entities;
using MediatR;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Users
{
    public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, UserResponse>
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _uow;

        public CreateUserHandler(IUserRepository users, IUnitOfWork uow)
        {
            _users = users;
            _uow = uow;
        }

        public async Task<UserResponse> Handle(CreateUserCommand req, CancellationToken ct)
        {
            if (await _users.EmailExistsAsync(req.Request.Email, ct))
                throw new InvalidOperationException("EMAIL_ALREADY_EXISTS");

            var entity = User.Create(req.Request.FirstName, req.Request.LastName, req.Request.Email, req.Request.Address);
            await _users.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return entity.Adapt<UserResponse>();
        }
    }
}
