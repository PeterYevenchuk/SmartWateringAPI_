using FluentValidation;

namespace SmartWatering.Core.CityInfo.ChangeCityInfo;

public class ChangeCityInfoCommandValidator : AbstractValidator<ChangeCityInfoCommand>
{
    public ChangeCityInfoCommandValidator()
    {
        RuleFor(a => a.CityName).NotEmpty().NotNull().MaximumLength(200);
        RuleFor(a => a.UserId).NotEmpty().NotNull().GreaterThan(0);
    }
}
