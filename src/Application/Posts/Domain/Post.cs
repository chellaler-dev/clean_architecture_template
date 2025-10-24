namespace Application.Domain
{

        public class User
    {
        public int Id { get; set; }
        public Guid ReferenceId { get; private init; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string DisplayName { get; set; }
        public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
        public List<Post> Posts { get; init; } = new();
    }
    public class Post 
    {
        public int Id { get; set; }
        public Guid ReferenceId { get; private init; } = Guid.NewGuid();
        public required string Title { get; set; }
        public string? Content { get; set; }
        public required int UserId { get; init; }
        public User User { get; init; } = null!;
        public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
