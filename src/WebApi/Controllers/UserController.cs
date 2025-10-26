using Application.Users.Create;
using Application.Users.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace WebApi.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a post by its ID.
    /// </summary>
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetPostById(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);

        Result<UserResponse> result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return NotFound(result.Error);
    }


    /// <summary>
    /// Add a new post.
    /// </summary>
    [HttpPost("")]
    public async Task<IActionResult> AddUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Name,
            request.HasPublicProfile);

        Result<Guid> result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            // Return 201 Created with the new user's ID
            return Created();
        }

        // Handle failure — you can customize the status based on the error type
        return BadRequest(result.Error);
    }
}
