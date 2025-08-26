using Application.Commands.Users;
using Application.Contracts.Users;
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
    public sealed class UpdateUserHandler : IRequestHandler<UpdateUSerCommand, UserResponse>
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _uow;

        public UpdateUserHandler(IUserRepository users, IUnitOfWork uow)
        {
            _users = users;
            _uow = uow;
        }

        public async Task<UserResponse> Handle(UpdateUSerCommand req, CancellationToken ct)
        {
            var dto = req.Request;
            var e = await _users.GetByIdAsync(dto.Id, ct) ?? throw new KeyNotFoundException("USER_NOT_FOUND");

            if (!string.Equals(e.Email.Value, dto.Email, StringComparison.OrdinalIgnoreCase) &&
                await _users.EmailExistsAsync(dto.Email, ct))
                throw new InvalidOperationException("EMAIL_ALREADY_EXISTS");

            e.Update(dto.FirstName, dto.LastName, dto.Email, dto.Address);
            await _uow.SaveChangesAsync(ct);

            return e.Adapt<UserResponse>();
        }
    }
}
