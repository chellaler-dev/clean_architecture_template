using Application.Database;
using Application.Domain;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public static class AddPost
    {
        public sealed class AddPostValidator : AbstractValidator<Command>
        {
            public AddPostValidator()
            {
                RuleFor(c => c.Title).NotEmpty().WithMessage("Title is required!");
                RuleFor(c => c.UserId).GreaterThan(0).WithMessage("UserId must be valid!");
            }
        }

        // Command
        public record Command(string Title, string? Content, int UserId) : IRequest<Response>;

        // Handler
        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly Repository _repository;

            public Handler(Repository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = _repository.Users.FirstOrDefault(u => u.Id == request.UserId);
                if (user == null)
                    throw new InvalidOperationException($"User with Id {request.UserId} not found.");

                var newId = _repository.Posts.Max(p => p.Id) + 1;

                var post = new Post
                {
                    Id = newId,
                    Title = request.Title,
                    Content = request.Content,
                    UserId = user.Id,
                    User = user
                };

                _repository.Posts.Add(post);
                user.Posts.Add(post);

                return new Response { Id = post.Id, ReferenceId = post.ReferenceId };
            }
        }

        // Response
        public record Response
        {
            public int Id { get; init; }
            public Guid ReferenceId { get; init; }
        }
    }
}
