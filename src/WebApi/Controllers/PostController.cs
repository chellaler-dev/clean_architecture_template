using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a post by its ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var response = await _mediator.Send(new GetPostById.Query(id));
        return response == null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Add a new post.
    /// </summary>
    [HttpPost("")]
    public async Task<IActionResult> AddPost([FromBody] AddPost.Command command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
