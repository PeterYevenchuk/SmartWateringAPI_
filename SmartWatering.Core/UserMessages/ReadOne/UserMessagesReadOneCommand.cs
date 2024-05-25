using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserMessages.UpdateOne;

public class UserMessagesReadOneCommand : IRequest<IResult<Unit>>
{
    public int MessageId { get; set; }
}
