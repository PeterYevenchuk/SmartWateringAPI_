using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.SetSprinkler;

public class SetSprinklerCommand : IRequest<IResult<Unit>>
{
    public int UserId { get; set; }

    public string SprinklerNameId { get; set; }
}
