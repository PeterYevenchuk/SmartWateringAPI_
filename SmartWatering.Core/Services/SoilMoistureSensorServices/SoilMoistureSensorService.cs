using Newtonsoft.Json;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Models.SoilMoistureSensorData;

namespace SmartWatering.Core.Services.SoilMoistureSensorServices;

public class SoilMoistureSensorService : ISoilMoistureSensorService
{
    private readonly HttpClient _httpClient;
    private readonly IExecutionResult<WateringInformation> _executionResult;

    public SoilMoistureSensorService(HttpClient httpClient, IExecutionResult<WateringInformation> executionResult)
    {
        _executionResult = executionResult;
        _httpClient = httpClient;
    }

    public async Task<IResult<WateringInformation>> GetSoilMoistureSensorData()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7211/api/SoilMoistureSensor/GetSoilMoisture");

        if (response.IsSuccessStatusCode)
        {
            string responseData = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<WateringInformation>(responseData);
            if (data == null)
            {
                return await _executionResult.Fail("Data null!");
            }
            return await _executionResult.Successful(data);
        }

        return await _executionResult.Fail("Error communicating with the remote server");
    }

    public async Task<IResult<WateringInformation>> SetTurnOnStatus(bool turnOn, int userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsync($"https://localhost:7211/api/SoilMoistureSensor/set-turn-on-status/{turnOn}/{userId}", null);

        if (response.IsSuccessStatusCode)
        {
            return await _executionResult.Successful(null);
        }

        return await _executionResult.Fail("Error communicating with the remote server");
    }
}
