using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.CityInfo.ChangeCityInfo;

public class ChangeCityInfoCommand : IRequest<IResult<Unit>>
{
    public int UserId { get; set; }

    public string CityName { get; set; }
}
