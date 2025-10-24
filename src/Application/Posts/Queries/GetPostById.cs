using Application.Database;
using MediatR;

namespace Application.Queries
{
    public static class GetPostById
    {
        // Query
        public record Query(int Id) : IRequest<Response>;

        // Handler
        public class Handler : IRequestHandler<Query, Response?>
        {
            private readonly Repository _repository;

            public Handler(Repository repository)
            {
                _repository = repository;
            }

            public async Task<Response?> Handle(Query request, CancellationToken cancellationToken)
            {
                var post = _repository.Posts.FirstOrDefault(x => x.Id == request.Id);

                return post == null
                    ? null
                    : new Response
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,
                        UserId = post.UserId,
                        UserDisplayName = post.User.DisplayName,
                        CreatedAtUtc = post.CreatedAtUtc
                    };
            }
        }

        // Response
        public record Response
        {
            public int Id { get; init; }
            public string Title { get; init; } = string.Empty;
            public string? Content { get; init; }
            public int UserId { get; init; }
            public string UserDisplayName { get; init; } = string.Empty;
            public DateTime CreatedAtUtc { get; init; }
        }
    }
}
