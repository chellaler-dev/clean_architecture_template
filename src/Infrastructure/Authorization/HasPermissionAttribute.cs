using Domain.Permissions;

namespace Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : Attribute
{
    public HasPermissionAttribute(PermissionEnum permission)
    {
        Permission = permission.ToString();
    }

    public string Permission { get; }
}