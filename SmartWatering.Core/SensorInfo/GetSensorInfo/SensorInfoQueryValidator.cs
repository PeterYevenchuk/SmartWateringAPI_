using FluentValidation;

namespace SmartWatering.Core.SensorInfo.GetSensorInfo;

public class SensorInfoQueryValidator : AbstractValidator<SensorInfoQuery>
{
    public SensorInfoQueryValidator()
    {
        RuleFor(a => a.UserId).NotEmpty().NotNull().GreaterThan(0);
    }
}
