using AutoMapper;
using MediatR;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Helpers.PasswordHasher;
using SmartWatering.Core.Services.WeatherService;
using SmartWatering.DAL.DbContext;
using SmartWatering.DAL.Models;

namespace SmartWatering.Core.UserInfo.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult<Unit>>
{
    private readonly IExecutionResult<Unit> _executionResult;
    private readonly SwDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWeatherService _weatherService;
    private readonly IPasswordHash _passwordHash;

    public CreateUserCommandHandler(IExecutionResult<Unit> executionResult, SwDbContext context, IMapper mapper, IWeatherService weatherService,
        IPasswordHash passwordHash)
    {
        _executionResult = executionResult;
        _weatherService = weatherService;
        _context = context;
        _mapper = mapper;
        _passwordHash = passwordHash;
    }

    public async Task<IResult<Unit>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
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
                var user = _mapper.Map<User>(request);

                var salt = GenerateSalt.GenerateRandomSalt(16);
                var hashPassword = _passwordHash.EncryptPassword(request.Password, salt);

                user.Password = hashPassword;
                user.Salt = Convert.ToBase64String(salt);

                var city = new City
                {
                    CityName = request.CityName,
                    UserId = user.Id,
                    User = user
                };

                _context.Users.Add(user);
                _context.Cities.Add(city);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await _executionResult.Successful(Unit.Value);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await _executionResult.Fail($"Failed to create user. Error: {ex.Message}");
            }
        }
    }
}
