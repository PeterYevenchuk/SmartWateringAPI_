using FluentValidation;

namespace SmartWatering.Core.UserInfo.SetSprinkler;

public class SetSprinklerCommandValidator : AbstractValidator<SetSprinklerCommand>
{
    public SetSprinklerCommandValidator()
    {
        RuleFor(s => s.UserId).NotEmpty().NotNull().GreaterThan(0);
        RuleFor(s => s.SprinklerNameId).NotEmpty().NotNull();
    }
}
