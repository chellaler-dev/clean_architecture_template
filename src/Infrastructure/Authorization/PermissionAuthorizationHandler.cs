using Microsoft.AspNetCore.Authorization;
using IAuthorizationService = Application.Abstractions.Authorization.IAuthorizationService;

namespace Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler 
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly Application.Abstractions.Authorization.IAuthorizationService _authorizationService;

    public PermissionAuthorizationHandler(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == "userId");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return;
        }

        var hasPermission = await _authorizationService
            .HasPermissionAsync(userId, requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

    }
}