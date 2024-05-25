using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserMessages.DeleteOne;

public class UserMessageDeleteOneCommandHandler : IRequestHandler<UserMessageDeleteOneCommand, IResult<Unit>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<Unit> _executionResult;

    public UserMessageDeleteOneCommandHandler(SwDbContext context, IExecutionResult<Unit> executionResult)
    {
        _context = context;
        _executionResult = executionResult;
    }
    public async Task<IResult<Unit>> Handle(UserMessageDeleteOneCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == request.MessageId);

        if (message == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(message)).ToString());
        }
        _context.Messages.Remove(message);
        _context.SaveChanges();

        return await _executionResult.Successful(Unit.Value);
    }
}
