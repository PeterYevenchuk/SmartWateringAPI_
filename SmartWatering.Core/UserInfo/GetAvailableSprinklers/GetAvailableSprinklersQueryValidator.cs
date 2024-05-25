using FluentValidation;

namespace SmartWatering.Core.UserInfo.GetAvailableSprinklers;

public class GetAvailableSprinklersQueryValidator : AbstractValidator<GetAvailableSprinklersQuery>
{
    public GetAvailableSprinklersQueryValidator()
    {
        RuleFor(s => s.UserId).NotEmpty().NotNull().GreaterThan(0);
    }
}
