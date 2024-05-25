using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Models.SoilMoistureSensorData;

namespace SmartWatering.Core.Services.SoilMoistureSensorServices;

public interface ISoilMoistureSensorService
{
    public Task<IResult<WateringInformation>> GetSoilMoistureSensorData();

    public Task<IResult<WateringInformation>> SetTurnOnStatus(bool turnOn, int userId);
}
