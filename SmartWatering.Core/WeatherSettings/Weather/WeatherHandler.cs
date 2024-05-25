using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Models.OpenWeatherData;
using SmartWatering.Core.Models.SoilMoistureSensorData;
using SmartWatering.Core.Services.SoilMoistureSensorServices;
using SmartWatering.Core.Services.WeatherService;
using SmartWatering.Core.WeatherSettings.Messages;
using SmartWatering.DAL.DbContext;
using SmartWatering.DAL.Models;
using SmartWatering.DAL.Models.TestSprinkler;
using System.Globalization;

namespace SmartWatering.Core.WeatherSettings.Weather;

public class WeatherHandler : IWeatherHandler
{
    private readonly IWeatherService _weatherService;
    private readonly ISoilMoistureSensorService _soilMoistureSensorService;
    private readonly IExecutionResult<string> _executionResult;
    private readonly IExecutionResult<ResultMessage> _executionResultMessage;
    private readonly IExecutionResult<List<ResultMessage>> _executionListResultMessage;
    private readonly SwDbContext _context;
    private readonly IMapper _mapper;

    public WeatherHandler(IWeatherService weatherService, SwDbContext context, IExecutionResult<string> executionResult,
        ISoilMoistureSensorService soilMoistureSensorService, IExecutionResult<ResultMessage> executionResultMessage,
        IExecutionResult<List<ResultMessage>> executionListResultMessage, IMapper mapper)
    {
        _soilMoistureSensorService = soilMoistureSensorService;
        _executionResult = executionResult;
        _weatherService = weatherService;
        _context = context;
        _mapper = mapper;
        _executionResultMessage = executionResultMessage;
        _executionListResultMessage = executionListResultMessage;
    }

    public async Task<IResult<List<ResultMessage>>> WeatherRecomendation(bool tomorrow)
    {
        List<ResultMessage> messages = new List<ResultMessage>();
        var wateringsUser = _context.Waterings.Where(w => w.SprinklerNameId == Sprinkler.NameId).ToList();

        if (wateringsUser == null)
        {
            return await _executionListResultMessage.Fail(new ArgumentNullException(nameof(wateringsUser)).ToString());
        }

        foreach (var wateringUser in wateringsUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == wateringUser.UserId);

            if (user == null)
            {
                return await _executionListResultMessage.Fail(new ArgumentNullException(nameof(user)).ToString());
            }

            var city = await _context.Cities.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (city == null)
            {
                return await _executionListResultMessage.Fail(new ArgumentNullException(nameof(city)).ToString());
            }

            var weatherData = await _weatherService.GetWeatherAsync(city.CityName);

            if (!weatherData.IsSuccess || weatherData.Data == null)
            {
                return await _executionListResultMessage.Fail(weatherData.ErrorMessage);
            }

            var soilMoistureData = await _soilMoistureSensorService.GetSoilMoistureSensorData();

            if (!soilMoistureData.IsSuccess || soilMoistureData.Data == null)
            {
                return await _executionListResultMessage.Fail(soilMoistureData.ErrorMessage);
            }

            /*        TimeSpan timeZoneOffset = TimeSpan.FromSeconds(weatherData.Data.City.Timezone);
                    TimeZoneInfo cityTimeZone = TimeZoneInfo.CreateCustomTimeZone("TimeZone", timeZoneOffset, null, null);
                    DateTime currentDateTimeInCity = TimeZoneInfo.ConvertTime(DateTime.Now, cityTimeZone);*/

            var message = await GetRecommendation(weatherData.Data, soilMoistureData.Data, DateTime.Now, tomorrow, user);

            if (!message.IsSuccess)
            {
                return await _executionListResultMessage.Fail(message.ErrorMessage);
            }

            message.Data.UserId = user.Id;
            messages.Add(message.Data);
            var resultMes = _mapper.Map<MessageModel>(message.Data);
            await _context.Messages.AddAsync(resultMes);
        }

        _context.SaveChanges();
        return await _executionListResultMessage.Successful(messages);
    }

    private async Task<IResult<ResultMessage>> GetRecommendation(ForecastData forecastData, WateringInformation soilMoisture, 
        DateTime targetDate, bool tomorrow, User user)
    {
        var getRecomendation = await GetRecommendation(null, null, null);
        var result = new ResultMessage();
        List<WeatherForecast> relevantForecasts;
        var forecastDataToday = forecastData.List.Where(entry => DateTime.Parse(entry.Dt_txt).Day == targetDate.Day).ToList();

        // morning
        #region morning
        if (targetDate.Hour >= 6 && targetDate.Hour < 12 && targetDate.Day == DateTime.Today.Day && tomorrow == false)
        {
            relevantForecasts = forecastDataToday
                .Where(entry => DateTime.Parse(entry.Dt_txt).Hour >= 6 && DateTime.Parse(entry.Dt_txt).Hour < 12)
                .ToList();

            if (relevantForecasts.Count <= 0)
            {
                return await _executionResultMessage.Fail(new ArgumentNullException(nameof(relevantForecasts)).ToString());
            }

            if (targetDate.Hour >= 6 && targetDate.Hour < 9)
            {
                var morningForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 6 && DateTime.Parse(entry.Dt_txt).Hour < 9);
                getRecomendation = await GetRecommendation(morningForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Morning,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }

            if (targetDate.Hour >= 9 && targetDate.Hour < 12)
            {
                var midMorningForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 9 && DateTime.Parse(entry.Dt_txt).Hour < 12);
                getRecomendation = await GetRecommendation(midMorningForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Morning,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }
        }
        #endregion

        // day
        #region day
        if (targetDate.Hour >= 12 && targetDate.Hour < 18 && targetDate.Day == DateTime.Today.Day && tomorrow == false)
        {
            relevantForecasts = forecastDataToday
                .Where(entry => DateTime.Parse(entry.Dt_txt).Hour >= 12 && DateTime.Parse(entry.Dt_txt).Hour < 18)
                .ToList();

            if (relevantForecasts.Count <= 0)
            {
                return await _executionResultMessage.Fail(new ArgumentNullException(nameof(relevantForecasts)).ToString());
            }

            if (targetDate.Hour >= 12 && targetDate.Hour < 15)
            {
                var dayForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 12 && DateTime.Parse(entry.Dt_txt).Hour < 15);
                getRecomendation = await GetRecommendation(dayForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Day,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }

            if (targetDate.Hour >= 15 && targetDate.Hour < 18)
            {
                var midDayForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 15 && DateTime.Parse(entry.Dt_txt).Hour < 18);
                getRecomendation = await GetRecommendation(midDayForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Day,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }
        }
        #endregion

        // evening
        #region evening
        if (targetDate.Hour >= 18 && (targetDate.Hour < 23 && targetDate.Minute < 59 && targetDate.Second < 59) && targetDate.Day == DateTime.Today.Day && tomorrow == false)
        {
            relevantForecasts = forecastDataToday
                .Where(entry => DateTime.Parse(entry.Dt_txt).Hour >= 18 && (DateTime.Parse(entry.Dt_txt).Hour < 23 && targetDate.Minute < 59 && targetDate.Second < 59))
                .ToList();

            if (relevantForecasts.Count <= 0)
            {
                return await _executionResultMessage.Fail(new ArgumentNullException(nameof(relevantForecasts)).ToString());
            }

            if (targetDate.Hour >= 18 && targetDate.Hour < 21)
            {
                var eveningForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 18 && DateTime.Parse(entry.Dt_txt).Hour < 21);
                getRecomendation = await GetRecommendation(eveningForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Evening,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }

            if (targetDate.Hour >= 21 && (targetDate.Hour < 23 && targetDate.Minute < 59 && targetDate.Second < 59))
            {
                var midEveningForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 21 && (DateTime.Parse(entry.Dt_txt).Hour < 23 && targetDate.Minute < 59 && targetDate.Second < 59));
                getRecomendation = await GetRecommendation(midEveningForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Evening,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }
        }
        #endregion

        // night
        #region night
        if (targetDate.Hour >= 0 && targetDate.Hour < 6 && targetDate.Day == DateTime.Today.Day && tomorrow == false)
        {
            relevantForecasts = forecastDataToday
                .Where(entry => DateTime.Parse(entry.Dt_txt).Hour >= 0 && DateTime.Parse(entry.Dt_txt).Hour < 6)
                .ToList();

            if (relevantForecasts.Count <= 0)
            {
                return await _executionResultMessage.Fail(new ArgumentNullException(nameof(relevantForecasts)).ToString());
            }

            if (targetDate.Hour >= 0 && targetDate.Hour < 3)
            {
                var nightForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 00 && DateTime.Parse(entry.Dt_txt).Hour < 3);
                getRecomendation = await GetRecommendation(nightForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Night,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }

            if (targetDate.Hour >= 3 && targetDate.Hour < 6)
            {
                var midNightForecast = relevantForecasts
                    .First(entry => DateTime.Parse(entry.Dt_txt).Hour >= 3 && DateTime.Parse(entry.Dt_txt).Hour < 6);
                getRecomendation = await GetRecommendation(midNightForecast, soilMoisture, user);
                result = new ResultMessage
                {
                    DayTime = DayTimeZone.Night,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = getRecomendation.Data
                };
                return await _executionResultMessage.Successful(result);
            }
        }
        #endregion

        // tomorrow
        #region tomorrow
        if (tomorrow == true)
        {
            relevantForecasts = forecastData.List
                .Where(entry => DateTime.TryParseExact(entry.Dt_txt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)
                        && date.Date == targetDate.Date.AddDays(1)).ToList();

            if (relevantForecasts.Count <= 0)
            {
                return await _executionResultMessage.Fail(new ArgumentNullException(nameof(relevantForecasts)).ToString());
            }

            double averageTemperature = relevantForecasts.Average(entry => entry.Main.Temp);
            double averageHumidity = relevantForecasts.Average(entry => entry.Main.Humidity);
            double averageWindSpeed = relevantForecasts.Average(entry => entry.Wind.Speed);

            getRecomendation = await GetTomorrowRecommendation(averageTemperature, averageHumidity, averageWindSpeed);
            result = new ResultMessage
            {
                DayTime = DayTimeZone.TomorrowAllDay,
                DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                Message = getRecomendation.Data
            };
            return await _executionResultMessage.Successful(result);
        }
        #endregion

        return await _executionResultMessage.Fail(RecomendationMessages.M0000);
    }

    private async Task<IResult<string>> GetTomorrowRecommendation(double averageTemperature, double averageHumidity, double averageWindSpeed)
    {
        if (averageTemperature < 0 || averageHumidity < 0 || averageWindSpeed < 0)
        {
            return await _executionResult.Fail(new ArgumentNullException("Values cannot be less than 0!").ToString());
        }

        if (averageTemperature > 25)
        {
            return await _executionResult.Successful(RecomendationMessages.M0101);
        }
        else if (averageTemperature >= 15 && averageTemperature <= 25)
        {
            return await _executionResult.Successful(RecomendationMessages.M0102);
        }
        else if (averageTemperature < 10)
        {
            return await _executionResult.Successful(RecomendationMessages.M0103);
        }
        else if(averageWindSpeed > 15)
        {
            return await _executionResult.Successful(RecomendationMessages.M0104);
        }
        else if ((averageTemperature <= 18 || averageTemperature >= 25) && (averageHumidity <= 50 || averageHumidity >= 70))
        {
            return await _executionResult.Successful(RecomendationMessages.M0105);
        }
        else
        {
            return await _executionResult.Successful(RecomendationMessages.M0106);
        }
    }

    private async Task<IResult<string>> GetRecommendation(WeatherForecast relevantForecast, WateringInformation soilMoisture, User user)
    {
        if (relevantForecast == null || soilMoisture == null)
        {
            return await _executionResult.Fail(new ArgumentNullException("RelevantForecast or soilMoisture is null!").ToString());
        }

        var weatherConditionResult = await GetWeatherCondition(relevantForecast, soilMoisture.HumidityData, user);

        if (weatherConditionResult.IsSuccess)
        {
            return await _executionResult.Successful(weatherConditionResult.Data);
        }
        else if(relevantForecast.Main.Temp > 25 && relevantForecast.Main.Humidity < 70 && soilMoisture.HumidityData < 40)
        {
            if (user.AutoMode == true)
            {
                await _soilMoistureSensorService.SetTurnOnStatus(true, user.Id);
            }
            return await _executionResult.Successful(RecomendationMessages.M0001);
        }
        else if (relevantForecast.Main.Temp > 30 && soilMoisture.HumidityData < 40)
        {
            if (user.AutoMode == true)
            {
                await _soilMoistureSensorService.SetTurnOnStatus(true, user.Id);
            }
            return await _executionResult.Successful(RecomendationMessages.M0010);
        }
        else if (relevantForecast.Main.Temp > 35)
        {
            return await _executionResult.Successful(RecomendationMessages.M0011);
        }
        else if (relevantForecast.Main.Temp < 5 && soilMoisture.HumidityData < 30)
        {
            return await _executionResult.Successful(RecomendationMessages.M0007);
        }
        else if (relevantForecast.Main.Temp < 0 && soilMoisture.HumidityData > 40)
        {
            return await _executionResult.Successful(RecomendationMessages.M0013);
        }
        else if (relevantForecast.Main.Humidity > 70 && soilMoisture.HumidityData > 60)
        {
            return await _executionResult.Successful(RecomendationMessages.M0009);
        }
        else if (relevantForecast.Main.Temp >= 18 && relevantForecast.Main.Temp <= 25 && soilMoisture.HumidityData < 40)
        {
            if (user.AutoMode == true)
            {
                await _soilMoistureSensorService.SetTurnOnStatus(true, user.Id);
            }
            return await _executionResult.Successful(RecomendationMessages.M0012);
        }
        else if (soilMoisture.HumidityData <= 30 && relevantForecast.Main.Temp >= 10)
        {
            if (user.AutoMode == true)
            {
                await _soilMoistureSensorService.SetTurnOnStatus(true, user.Id);
            }
            return await _executionResult.Successful(RecomendationMessages.M0016);
        }
        else
        {
            return await _executionResult.Successful(RecomendationMessages.M0006);
        }
    }

    private async Task<IResult<string>> GetWeatherCondition(WeatherForecast relevantForecast, double soilMoistureLevel, User user)
    {
        if (IsRainyWeather(relevantForecast, soilMoistureLevel))
        {
            return await _executionResult.Successful(RecomendationMessages.M0002);
        }
        else if (IsThunderstorm(relevantForecast, soilMoistureLevel))
        {
            return await _executionResult.Successful(RecomendationMessages.M0005);
        }
        else if (IsDrizzle(relevantForecast, soilMoistureLevel))
        {
            return await _executionResult.Successful(RecomendationMessages.M0014);
        }
        else if (IsSnow(relevantForecast))
        {
            return await _executionResult.Successful(RecomendationMessages.M0004);
        }
        else if (IsAtmosphere(relevantForecast))
        {
            if (soilMoistureLevel >= 50)
            {
                return await _executionResult.Successful(RecomendationMessages.M0008);
            }
            else
            {
                if (user.AutoMode == true && soilMoistureLevel <= 30)
                {
                    await _soilMoistureSensorService.SetTurnOnStatus(true, user.Id);
                }
                return await _executionResult.Successful(RecomendationMessages.M0015);
            }
        }

        return await _executionResult.Fail("");
    }

    private bool IsRainyWeather(WeatherForecast relevantForecast, double soilMoistureLevel)
    {
        return relevantForecast.Weather[0].Main.ToLower() == "rain" && soilMoistureLevel > 50 && relevantForecast.Clouds.All > 50;
    }

    private bool IsSnow(WeatherForecast relevantForecast)
    {
        return relevantForecast.Weather[0].Main.ToLower() == "snow";
    }

    private bool IsThunderstorm(WeatherForecast relevantForecast, double soilMoistureLevel)
    {
        return relevantForecast.Weather[0].Main.ToLower() == "thunderstorm" && relevantForecast.Clouds.All > 80 && soilMoistureLevel > 50;
    }

    private bool IsDrizzle(WeatherForecast relevantForecast, double soilMoistureLevel)
    {
        return relevantForecast.Weather[0].Main.ToLower() == "drizzle" && soilMoistureLevel > 50 && relevantForecast.Clouds.All > 40;
    }

    private bool IsAtmosphere(WeatherForecast relevantForecast)
    {
        return relevantForecast.Weather[0].Id >= 701 && relevantForecast.Weather[0].Id <= 781;
    }
}
