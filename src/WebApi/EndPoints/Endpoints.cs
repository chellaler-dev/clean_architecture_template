using Application.Users.Create;
using Application.Users.GetById;
using Application.Users.Login;
using MediatR;
using SharedKernel;

namespace WebApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {

        app.MapGet("users/{userId}", async (
            Guid userId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserByIdQuery(userId);

            Result<UserResponse> result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return Results.NotFound();
        }).RequireAuthorization();
       
       
       
        // Create User
        app.MapPost("users", async (
            CreateUserRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateUserCommand(
                request.Email,
                request.Name,
                request.HasPublicProfile);

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Created($"users/{result.Value}", null)
                : Results.BadRequest(result.Error);
        });

        // Login
        app.MapPost("auth/login", async (
            LoginRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Email);

            Result<string> result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(new { Token = result.Value })
                : Results.BadRequest(result.Error);
        });
    }
}
