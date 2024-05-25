using FluentValidation;

namespace SmartWatering.Core.UserInfo.GetUserInformation;

public class GetUserInformationQueryValidator : AbstractValidator<GetUserInformationQuery>
{
    public GetUserInformationQueryValidator()
    {
        RuleFor(u => u.UserId).NotNull().NotEmpty().GreaterThan(0);
    }
}
