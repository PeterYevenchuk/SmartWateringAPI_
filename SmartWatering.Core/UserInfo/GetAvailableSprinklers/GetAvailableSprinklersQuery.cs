using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.GetAvailableSprinklers;

public class GetAvailableSprinklersQuery : IRequest<IResult<IEnumerable<WateringDTO>>>
{
    public int UserId { get; set; }
}
