namespace Application.Abstractions.Authorization;

public interface IAuthorizationService
{
    public Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
}