using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.WateringAutoMode;

public class WateringAutoModeCommand : IRequest<IResult<Unit>>
{
    public int Id { get; set; }

    public bool TurnOn { get; set; }
}
