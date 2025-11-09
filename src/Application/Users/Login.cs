using Application.Abstractions.Messaging;
using Application.Abstractions.Authentication;
using Domain.Users;
using SharedKernel;
using FluentValidation;

namespace Application.Users.Login;

public sealed record LoginRequest(string Email);
public sealed record LoginCommand(string Email) : ICommand<string>; // Returns JWT token

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithErrorCode(UserErrorCodes.Login.MissingEmail)
            .EmailAddress().WithErrorCode(UserErrorCodes.Login.InvalidEmail);
    }
}


internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IJwtProvider jwtProvider) : ICommandHandler<LoginCommand, string>
{
    public async Task<Result<string>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure<string>(emailResult.Error);

        var email = emailResult.Value;

        User? user = await userRepository.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<string>(UserErrors.NotFoundByEmail);

        string token = jwtProvider.Generate(user);

        return token;
    }
}
