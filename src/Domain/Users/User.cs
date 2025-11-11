using Domain.Permissions;
using SharedKernel;

namespace Domain.Users;

/// <summary>
/// Represents a rich domain entity for a User, encapsulating both data and behavior.
/// Ensures invariants through private setters and factory methods, uses value objects
/// for strongly-typed properties, and raises domain events to signal important state changes.
/// </summary>
public sealed class User : Entity
{
    // Private constructor
    // Ensures that instances are always valid when created.
    private User(Guid id, Email email, Name name, bool hasPublicProfile)
        : base(id)
    {
        Email = email;
        Name = name;
        HasPublicProfile = hasPublicProfile;
        Roles = new List<Role>();
    }

    // Parameterless constructor required by ORMs (like EF Core) for materialization.
    private User()
    {
        Roles = new List<Role>();
    }

    // Properties (Value Objects) of the User. Some are private set to enforce encapsulation and invariants.
    public Email Email { get; private set; }
    public Name Name { get; private set; }
    public bool HasPublicProfile { get; set; } // Can be changed publicly (domain decision)
    
    // Navigation property for authorization
    public ICollection<Role> Roles { get; private set; }

    // Factory method to create a new User in a valid state.
    // Encapsulates all the creation logic and invariants.
    public static User Create(Email email, Name name, bool hasPublicProfile)
    {
        var user = new User(Guid.NewGuid(), email, name, hasPublicProfile);

        // This decouples side-effects (like sending emails) from the entity itself.
        user.Raise(new UserCreatedDomainEvent(user.Id));

        return user;
    }

    // Domain method to assign a role to the user
    public void AssignRole(Role role)
    {
        if (!Roles.Any(r => r.Id == role.Id))
        {
            Roles.Add(role);
        }
    }

    // Domain method to remove a role from the user
    public void RemoveRole(Role role)
    {
        var existingRole = Roles.FirstOrDefault(r => r.Id == role.Id);
        if (existingRole is not null)
        {
            Roles.Remove(existingRole);
        }
    }
}