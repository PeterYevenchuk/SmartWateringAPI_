using FluentValidation;

namespace SmartWatering.Core.UserMessages.DeleteOne;

public class UserMessageDeleteOneCommandValidator : AbstractValidator<UserMessageDeleteOneCommand>
{
    public UserMessageDeleteOneCommandValidator()
    {
        RuleFor(m => m.MessageId).NotEmpty().NotNull().GreaterThan(0);
    }
}
