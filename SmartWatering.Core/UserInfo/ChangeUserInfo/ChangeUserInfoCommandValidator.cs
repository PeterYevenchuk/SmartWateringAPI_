using FluentValidation;

namespace SmartWatering.Core.UserInfo.ChangeUserInfo;

public class ChangeUserInfoCommandValidator : AbstractValidator<ChangeUserInfoCommand>
{
    public ChangeUserInfoCommandValidator()
    {
        RuleFor(a => a.Name).MaximumLength(100);
        RuleFor(a => a.Email).MaximumLength(200).EmailAddress();
        RuleFor(a => a.SurName).MaximumLength(150);
        RuleFor(a => a.Password).MaximumLength(100).MinimumLength(8);
        RuleFor(a => a.OldPassword).MaximumLength(100);
        RuleFor(a => a.Id).NotEmpty().NotNull().GreaterThan(0);
    }
}
