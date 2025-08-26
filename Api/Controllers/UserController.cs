using Microsoft.AspNetCore.Mvc;
using Application.Commands.Users;
using Application.Contracts.Users;
using Application.Queries.Users;
using MediatR;
using System;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<UserResponse> Create([FromBody] CreateUserRequest req, CancellationToken ct)
            => _mediator.Send(new CreateUserCommand(req), ct);

        [HttpPut("{id:guid}")]
        public Task<UserResponse> Update([FromRoute] Guid id, [FromBody] CreateUserRequest body, CancellationToken ct)
            => _mediator.Send(new UpdateUSerCommand(new UpdateUserRequest
            {
                Id = id,
                FirstName = body.FirstName,
                LastName = body.LastName,
                Email = body.Email,
                Address = body.Address
            }), ct);

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteUserCommand(id), ct);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public Task<UserResponse?> Get([FromRoute] Guid id, CancellationToken ct)
            => _mediator.Send(new GetUserByIdQuery(id), ct);

        [HttpGet]
        public Task<PagedResult<UserResponse>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
                                                    [FromQuery] string? search = null, CancellationToken ct = default)
            => _mediator.Send(new ListUsersQuery(page, pageSize, search), ct);
    }
}
