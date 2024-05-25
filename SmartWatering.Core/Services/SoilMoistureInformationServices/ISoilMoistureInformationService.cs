using MediatR;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Models.SoilMoistureSensorData;

namespace SmartWatering.Core.Services.SoilMoistureInformationServices;

public interface ISoilMoistureInformationService
{
    public Task<IResult<Unit>> SaveSoilMoistureInformation(SoilMoistureInformation data);
}
