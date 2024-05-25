using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserInfo.DeleteSprinkler;

public class DeleteSprinklerCommandHandler : IRequestHandler<DeleteSprinklerCommand, IResult<Unit>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<Unit> _executionResult;

    public DeleteSprinklerCommandHandler(SwDbContext context, IExecutionResult<Unit> executionResult)
    {
        _context = context;
        _executionResult = executionResult;
    }

    public async Task<IResult<Unit>> Handle(DeleteSprinklerCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var watering = await _context.Waterings.FirstOrDefaultAsync(w => w.Id == request.Id);

        if (watering == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(watering)).ToString());
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.Waterings.Remove(watering);

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
