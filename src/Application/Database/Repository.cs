using Application.Domain;

namespace Application.Database
{
    public class Repository
    {
        public List<User> Users { get; } = new()
        {
            new User
            {
                Id = 1,
                Username = "johndoe",
                Password = "password123",
                DisplayName = "John Doe"
            },
            new User
            {
                Id = 2,
                Username = "janedoe",
                Password = "securepass",
                DisplayName = "Jane Doe"
            }
        };

        public List<Post> Posts { get; }

        public Repository()
        {
            Posts = new List<Post>
            {
                new Post { Id = 1, Title = "My First Post", Content = "Hello world!", UserId = 1, User = Users[0] },
                new Post { Id = 2, Title = "Learning C#", Content = "C# is awesome!", UserId = 2, User = Users[1] },
                new Post { Id = 3, Title = "Daily Thoughts", Content = "Coding and coffee ☕", UserId = 1, User = Users[0] }
            };

            // Link posts to users
            Users[0].Posts.AddRange(Posts.Where(p => p.UserId == 1));
            Users[1].Posts.AddRange(Posts.Where(p => p.UserId == 2));
        }
    }
}
