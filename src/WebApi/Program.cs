using System.Text;
using Application;
using Application.Users.Create;
using Application.Users.GetById;
using Application.Users.Login;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedKernel;
using WebApi.ExceptionHandlers;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token obtained from login (without 'Bearer ' prefix)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});


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


// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]
                    ?? throw new InvalidOperationException("Missing Jwt:SecretKey"))),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

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
}).RequireAuthorization();

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


app.MapPost("auth/login", async (
    LoginRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var command = new LoginCommand(request.Email);

    Result<string> result = await sender.Send(command, cancellationToken);

    if (result.IsSuccess)
    {
        return Results.Ok(new { Token = result.Value });
    }

    return Results.BadRequest(result.Error);
});


//app.UseExceptionHandler();

app.Run();