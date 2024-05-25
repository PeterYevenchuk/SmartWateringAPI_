using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Models.OpenWeatherData;

namespace SmartWatering.Core.Services.WeatherService;

public interface IWeatherService
{
    public Task<IResult<ForecastData>> GetWeatherAsync(string city);
}