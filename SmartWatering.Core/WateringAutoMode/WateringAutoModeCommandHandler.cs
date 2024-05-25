using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Helpers.PasswordHasher;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.WateringAutoMode;

public class WateringAutoModeCommandHandler : IRequestHandler<WateringAutoModeCommand, IResult<Unit>>
{
    private readonly IExecutionResult<Unit> _executionResult;
    private readonly SwDbContext _context;
    private readonly IMapper _mapper;

    public WateringAutoModeCommandHandler(IExecutionResult<Unit> executionResult, SwDbContext context, IMapper mapper)
    {
        _executionResult = executionResult;
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResult<Unit>> Handle(WateringAutoModeCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

        if (user == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(user)).ToString());
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                user.AutoMode = request.TurnOn;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await _executionResult.Successful(Unit.Value);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await _executionResult.Fail($"Failed to change user info. Error: {ex.Message}");
            }
        }
    }
}
