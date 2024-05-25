using FluentValidation;

namespace SmartWatering.Core.UserInfo.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(a => a.Name).NotEmpty().NotNull().MaximumLength(100);
        RuleFor(a => a.Email).NotEmpty().NotNull().MaximumLength(200).EmailAddress();
        RuleFor(a => a.SurName).NotEmpty().NotNull().MaximumLength(150);
        RuleFor(a => a.Password).NotEmpty().NotNull().MaximumLength(100).MinimumLength(8);
        RuleFor(a => a.CityName).NotEmpty().NotNull().MaximumLength(200);
    }
}
