using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.SensorInfo.GetSensorInfo;

public class SensorInfoQuery : IRequest<IResult<IEnumerable<SensorInfoDTO>>>
{
    public int UserId { get; set; }
}
