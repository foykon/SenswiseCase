using Application.Commands.Users;
using Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Users
{
    public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _uow;

        public DeleteUserHandler(IUserRepository users, IUnitOfWork uow)
        {
            _users = users;
            _uow = uow;
        }

        public async Task<bool> Handle(DeleteUserCommand req, CancellationToken ct)
        {
            var e = await _users.GetByIdAsync(req.Id, ct) ?? throw new KeyNotFoundException("USER_NOT_FOUND");
            _users.Remove(e);
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
