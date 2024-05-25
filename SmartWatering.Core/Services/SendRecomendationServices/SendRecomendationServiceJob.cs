using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using SmartWatering.Core.Hubs;
using SmartWatering.Core.WeatherSettings.Weather;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.Services.SendRecomendationServices;

public class SendRecomendationServiceJob : IJob
{
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IWeatherHandler _weatherHandler;

    public SendRecomendationServiceJob(IHubContext<MessageHub> hubContext, IWeatherHandler weatherHandler)
    {
        _hubContext = hubContext;
        _weatherHandler = weatherHandler;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var data = await _weatherHandler.WeatherRecomendation(false);

        if (DateTime.Now.Hour >= 20 && DateTime.Now.Hour <= 23)
        {
            data = await _weatherHandler.WeatherRecomendation(true);
        }

        if (!data.IsSuccess)
        {
            throw new ArgumentException(data.ErrorMessage);
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", data.Data);
    }
}
