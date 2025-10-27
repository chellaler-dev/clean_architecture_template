using Application;
using Application.Users.Create;
using Application.Users.GetById;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using SharedKernel;
using WebApi.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);


// Extension methods used to add custom information whenever a problemDetails is instantiated in the application.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requesrId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

// Registers custom exception handlers for ValidationException and global exceptions.
// Ensure `app.UseExceptionHandler` is not used elsewhere to avoid conflicts.
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

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
});

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

    if (result.IsSuccess)
    {
        return Results.Created();
    }
    return Results.BadRequest(result.Error);
});


//app.UseExceptionHandler();

app.Run();