using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserMessages.Get;

public class UserMessagesQuery : IRequest<IResult<MessagesViewModel>>
{
    public int UserId { get; set; }
}
