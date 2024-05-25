using Microsoft.Extensions.Options;
using SmartWatering.Core.Models.OpenWeatherData;
using System.Net.Http.Json;
using SmartWatering.Core.Models.OpenWeatherSettings;
using SmartWatering.Core.ExecutionResults;
using System.Globalization;

namespace SmartWatering.Core.Services.WeatherService;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly IExecutionResult<ForecastData> _executionResult;

    public WeatherService(HttpClient httpClient, IOptions<AppSettings> appSettings, IExecutionResult<ForecastData> executionResult)
    {
        _httpClient = httpClient;
        _appSettings = appSettings.Value;
        _executionResult = executionResult;
    }

    public async Task<IResult<ForecastData>> GetWeatherAsync(string city)
    {
        try
        {
            var apiKey = _appSettings.OpenWeatherMapApiKey;
            var apiUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric";

            var response = await _httpClient.GetFromJsonAsync<ForecastData>(apiUrl);

            if (response == null)
            {
                return await _executionResult.Fail(new ArgumentNullException(nameof(response)).ToString());
            }

/*            var cityTimeZone = TimeZoneInfo.Utc;
            cityTimeZone = TimeZoneInfo.CreateCustomTimeZone("CityTimeZone", TimeSpan.FromSeconds(response.City.Timezone), null, null);
            if (cityTimeZone.BaseUtcOffset == TimeZoneInfo.Local.BaseUtcOffset)
            {
                return await _executionResult.Successful(response);
            }

            foreach (var forecast in response.List)
            {
                DateTime utcDateTime = DateTime.ParseExact(forecast.Dt_txt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                forecast.Dt_txt = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, cityTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
            }*/

            return await _executionResult.Successful(response);
        }
        catch (Exception ex)
        {
            return await _executionResult.Fail($"Internal Server Error: {ex.Message}");
        }
    }
}
