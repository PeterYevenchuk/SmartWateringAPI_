using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserMessages.DeleteOne;

public class UserMessageDeleteOneCommand : IRequest<IResult<Unit>>
{
    public int MessageId { get; set; }
}
