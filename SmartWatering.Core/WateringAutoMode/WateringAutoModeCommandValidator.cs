using FluentValidation;

namespace SmartWatering.Core.WateringAutoMode;

public class WateringAutoModeCommandValidator : AbstractValidator<WateringAutoModeCommand>
{
    public WateringAutoModeCommandValidator()
    {
        RuleFor(query => query.Id).NotNull().NotEmpty().GreaterThan(0);
    }
}
