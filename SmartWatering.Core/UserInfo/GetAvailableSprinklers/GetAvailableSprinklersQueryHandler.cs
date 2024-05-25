using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;
using SmartWatering.DAL.Models;

namespace SmartWatering.Core.UserInfo.GetAvailableSprinklers;

public class GetAvailableSprinklersQueryHandler : IRequestHandler<GetAvailableSprinklersQuery, IResult<IEnumerable<WateringDTO>>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<IEnumerable<WateringDTO>> _executionResult;
    private readonly IMapper _mapper;

    public GetAvailableSprinklersQueryHandler(SwDbContext context, IExecutionResult<IEnumerable<WateringDTO>> executionResult, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _executionResult = executionResult;
    }

    public async Task<IResult<IEnumerable<WateringDTO>>> Handle(GetAvailableSprinklersQuery request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var waterings = _context.Waterings.Where(w => w.UserId == request.UserId);

        if (waterings == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(waterings)).ToString());
        }

        var wateringDTOs = _mapper.Map<IEnumerable<WateringDTO>>(waterings);

        return await _executionResult.Successful(wateringDTOs);

    }
}
