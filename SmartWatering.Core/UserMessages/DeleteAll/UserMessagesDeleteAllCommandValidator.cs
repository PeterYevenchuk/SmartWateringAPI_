using FluentValidation;

namespace SmartWatering.Core.UserMessages.Delete;

public class UserMessagesDeleteAllCommandValidator : AbstractValidator<UserMessagesDeleteAllCommand>
{
    public UserMessagesDeleteAllCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty().NotNull().GreaterThan(0);
    }
}
