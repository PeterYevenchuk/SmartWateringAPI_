using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserMessages.ReadAll;

public class UserMessagesReadAllCommand : IRequest<IResult<Unit>>
{
    public int UserId { get; set; }
}
