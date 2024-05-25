using FluentValidation;

namespace SmartWatering.Core.UserMessages.Get;

public class UserMessagesQueryValidator : AbstractValidator<UserMessagesQuery>
{
    public UserMessagesQueryValidator()
    {
        RuleFor(m => m.UserId).NotEmpty().NotNull().GreaterThan(0);
    }
}
