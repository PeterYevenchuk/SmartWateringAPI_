using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.Models.OpenWeatherData;
using SmartWatering.Core.Services.WeatherService;
using SmartWatering.Core.WeatherSettings.Weather;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IWeatherHandler _weatherHandler;
    private readonly SwDbContext _context;

    public WeatherController(IWeatherService weatherService, IWeatherHandler weatherHandler, SwDbContext context)
    {
        _weatherService = weatherService;
        _weatherHandler = weatherHandler;
        _context = context;
    }

    [Authorize(Roles = "User")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetWeather(int id)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.UserId == id);

        if (city == null)
        {
            return StatusCode(404, nameof(city));
        }

        var weatherData = await _weatherService.GetWeatherAsync(city.CityName);

        if (weatherData.IsSuccess)
        {
            var targetDate = DateTime.Now;
            var forecastDataToday = weatherData.Data.List.Where(entry => DateTime.Parse(entry.Dt_txt).Day == targetDate.Day).ToList();
            return Ok(forecastDataToday);
        }
        else
        {
            return StatusCode(500, weatherData.ErrorMessage);
        }
    }

    [Authorize(Roles = "User")]
    [HttpGet("recomendation-messages/{tomorrow}")]
    public async Task<ActionResult> GetWeatherRecomendation(bool tomorrow)
    {
        var data = await _weatherHandler.WeatherRecomendation(tomorrow);
        return data.IsSuccess
        ? Ok(data.Data)
        : StatusCode(500, data.ErrorMessage);
    }
}

