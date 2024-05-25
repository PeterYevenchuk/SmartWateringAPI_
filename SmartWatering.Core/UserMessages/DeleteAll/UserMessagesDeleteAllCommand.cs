using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserMessages.Delete;

public class UserMessagesDeleteAllCommand : IRequest<IResult<Unit>>
{
    public int UserId { get; set; }
}
