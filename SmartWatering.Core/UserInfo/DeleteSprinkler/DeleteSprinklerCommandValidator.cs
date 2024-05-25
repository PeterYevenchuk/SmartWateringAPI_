using FluentValidation;

namespace SmartWatering.Core.UserInfo.DeleteSprinkler;

public class DeleteSprinklerCommandValidator :AbstractValidator<DeleteSprinklerCommand>
{
    public DeleteSprinklerCommandValidator()
    {
        RuleFor(s => s.Id).NotEmpty().NotNull().GreaterThan(0);
    }
}
