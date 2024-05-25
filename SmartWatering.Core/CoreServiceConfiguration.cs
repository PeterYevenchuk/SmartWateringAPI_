using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Helpers.PasswordHasher;
using SmartWatering.Core.Services.SoilMoistureInformationServices;
using SmartWatering.Core.Services.SoilMoistureSensorServices;
using SmartWatering.Core.Services.WeatherService;
using SmartWatering.Core.WeatherSettings.Weather;

namespace SmartWatering.Core;

public class CoreServiceConfiguration
{
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoreServiceConfiguration).Assembly))
            .AddAutoMapper(typeof(CoreMappingsProfile).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddScoped<IPasswordHash, PasswordHash>()
            .AddValidatorsFromAssembly(typeof(CoreServiceConfiguration).Assembly)
            .AddScoped<IWeatherService, WeatherService>()
            .AddScoped<ISoilMoistureSensorService, SoilMoistureSensorService>()
            .AddScoped<ISoilMoistureInformationService, SoilMoistureInformationService>()
            .AddScoped<IWeatherHandler, WeatherHandler>()
            .AddScoped(typeof(IExecutionResult<>), typeof(ExecutionResult<>));
    }
}
