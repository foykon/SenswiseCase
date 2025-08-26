using Application.Contracts.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users
{
    public sealed record UpdateUSerCommand(UpdateUserRequest Request) : IRequest<UserResponse>;

}
