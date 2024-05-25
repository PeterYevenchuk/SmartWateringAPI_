using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Helpers.PasswordHasher;
using SmartWatering.DAL.DbContext;
using SmartWatering.DAL.Models;

namespace SmartWatering.Core.UserInfo.SetSprinkler;

public class SetSprinklerCommandHandler : IRequestHandler<SetSprinklerCommand, IResult<Unit>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<Unit> _executionResult;

    public SetSprinklerCommandHandler(SwDbContext context, IExecutionResult<Unit> executionResult)
    {
        _context = context;
        _executionResult = executionResult;
    }

    public async Task<IResult<Unit>> Handle(SetSprinklerCommand request, CancellationToken cancellationToken)
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

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var data = new Watering
                {
                    UserId = request.UserId,
                    SprinklerNameId = request.SprinklerNameId
                };

                _context.Waterings.Add(data);

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
