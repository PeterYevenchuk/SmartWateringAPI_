using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.WeatherSettings.Messages;

namespace SmartWatering.Core.WeatherSettings.Weather;

public interface IWeatherHandler
{
    public Task<IResult<List<ResultMessage>>> WeatherRecomendation(bool tomorrow);
}
