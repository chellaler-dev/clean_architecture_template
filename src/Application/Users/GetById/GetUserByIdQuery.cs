using Application.Abstractions.Messaging;

namespace Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>
{
    public string CacheKey => $"user-by-id-{UserId}";

    public TimeSpan? Expiration => null;
}
