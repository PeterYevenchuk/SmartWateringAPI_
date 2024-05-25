using FluentValidation;

namespace SmartWatering.Core.UserMessages.UpdateOne;

public class UserMessagesReadOneCommandValidator : AbstractValidator<UserMessagesReadOneCommand>
{
    public UserMessagesReadOneCommandValidator()
    {
        RuleFor(m => m.MessageId).NotEmpty().NotNull().GreaterThan(0);
    }
}
