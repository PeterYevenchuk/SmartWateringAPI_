using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Services.SoilMoistureSensorServices;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserInfo.GetUserInformation;

public class GetUserInformationQueryHandler : IRequestHandler<GetUserInformationQuery, IResult<UserDTO>>
{
    private readonly SwDbContext _context;
    private readonly IMapper _mapper;
    private readonly IExecutionResult<UserDTO> _executionResult;
    private readonly ISoilMoistureSensorService _soilMoistureSensorService;

    public GetUserInformationQueryHandler(SwDbContext context, IMapper mapper, 
        IExecutionResult<UserDTO> executionResult, ISoilMoistureSensorService soilMoistureSensorService)
    {
        _context = context;
        _mapper = mapper;
        _executionResult = executionResult;
        _soilMoistureSensorService = soilMoistureSensorService;
    }

    public async Task<IResult<UserDTO>> Handle(GetUserInformationQuery request, CancellationToken cancellationToken)
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

        var data = await _soilMoistureSensorService.GetSoilMoistureSensorData();

        if (!data.IsSuccess)
        {
            return await _executionResult.Fail(data.ErrorMessage);
        }

        var result = _mapper.Map<UserDTO>(user);
        result.WateringInformation = data.Data;
        result.City = city.CityName;

        return await _executionResult.Successful(result);
    }
}
