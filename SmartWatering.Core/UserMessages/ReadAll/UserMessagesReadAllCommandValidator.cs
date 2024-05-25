using FluentValidation;

namespace SmartWatering.Core.UserMessages.ReadAll;

public class UserMessagesReadAllCommandValidator : AbstractValidator<UserMessagesReadAllCommand>
{
    public UserMessagesReadAllCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty().NotNull().GreaterThan(0);
    }
}
