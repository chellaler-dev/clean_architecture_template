using Application.Abstractions.Authorization;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

public sealed class AuthorizationService : IAuthorizationService
{
    private readonly ApplicationDbContext _context;

    public AuthorizationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(
        Guid userId, 
        string permission, 
        CancellationToken cancellationToken = default)
    {
        var permissions = await _context.Set<User>()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        return permissions.Contains(permission);
    }

}