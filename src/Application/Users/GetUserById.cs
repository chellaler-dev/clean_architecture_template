using Application.Abstractions.Caching;
using System.Data;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Dapper;
using Domain.Users;
using SharedKernel;

namespace Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : ICachedQuery<UserResponse>
{
    public string CacheKey => $"user-by-id-{UserId}";

    public TimeSpan? Expiration => null;
}

internal sealed class GetUserByIdQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT
                u.id AS Id,
                u.email AS Email,
                u.name AS Name,
                u.has_public_profile AS HasPublicProfile
            FROM users u
            WHERE u.id = @UserId
            """;

        using IDbConnection connection = factory.GetOpenConnection();

        UserResponse? user = await connection.QueryFirstOrDefaultAsync<UserResponse>(
            sql,
            query);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.UserId));
        }

        // It will be converted automatically to a Result<UserResponse> via the implicit operator defined in the Result<T> class.
        return user;
    }
}

public sealed record UserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string Name { get; init; }

    public bool HasPublicProfile { get; init; }
}