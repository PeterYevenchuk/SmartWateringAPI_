using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Services.WeatherService;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.CityInfo.ChangeCityInfo;

public class ChangeCityInfoCommandHandler : IRequestHandler<ChangeCityInfoCommand, IResult<Unit>>
{
    private readonly IExecutionResult<Unit> _executionResult;
    private readonly SwDbContext _context;
    private readonly IWeatherService _weatherService;

    public ChangeCityInfoCommandHandler(IExecutionResult<Unit> executionResult, SwDbContext context, IWeatherService weatherService)
    {
        _executionResult = executionResult;
        _context = context;
        _weatherService = weatherService;
    }

    public async Task<IResult<Unit>> Handle(ChangeCityInfoCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(user)).ToString());
        }

        var city = await _context.Cities.FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (city == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(city)).ToString());
        }

        var weatherResult = await _weatherService.GetWeatherAsync(request.CityName);

        if (!weatherResult.IsSuccess)
        {
            return await _executionResult.Fail($"City '{request.CityName}' does not exist.");
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                city.CityName = request.CityName ?? city.CityName;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await _executionResult.Successful(Unit.Value);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await _executionResult.Fail($"Failed to change city. Error: {ex.Message}");
            }
        }
    }
}
