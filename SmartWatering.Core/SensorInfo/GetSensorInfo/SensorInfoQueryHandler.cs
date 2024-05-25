using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.SensorInfo.GetSensorInfo;

public class SensorInfoQueryHandler : IRequestHandler<SensorInfoQuery, IResult<IEnumerable<SensorInfoDTO>>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<IEnumerable<SensorInfoDTO>> _executionResult;
    private readonly IMapper _mapper;

    public SensorInfoQueryHandler(SwDbContext context, IExecutionResult<IEnumerable<SensorInfoDTO>> executionResult, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _executionResult = executionResult;
    }

    public async Task<IResult<IEnumerable<SensorInfoDTO>>> Handle(SensorInfoQuery request, CancellationToken cancellationToken)
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

        var info = _context.SensorInformations.Where(s => s.UserId == request.UserId).ToList();

        if (info == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(info)).ToString());
        }

        return await _executionResult.Successful(_mapper.Map<IEnumerable<SensorInfoDTO>>(info));
    }
}
