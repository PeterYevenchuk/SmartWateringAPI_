using FluentValidation;

namespace SmartWatering.Core.UserInfo.Auth.AccessToken;

public class AuthCommandValidator : AbstractValidator<AuthCommand>
{
    public AuthCommandValidator()
    {
        RuleFor(query => query.Email).NotNull().NotEmpty();
        RuleFor(query => query.Password).NotNull().NotEmpty();
    }
}
