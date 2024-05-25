using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.DeleteSprinkler;

public class DeleteSprinklerCommand : IRequest<IResult<Unit>>
{
    public int Id { get; set; }
}
