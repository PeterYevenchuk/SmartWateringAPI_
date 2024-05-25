using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserMessages.Delete;

public class UserMessagesDeleteAllCommandHandler : IRequestHandler<UserMessagesDeleteAllCommand, IResult<Unit>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<Unit> _executionResult;

    public UserMessagesDeleteAllCommandHandler(SwDbContext context, IExecutionResult<Unit> executionResult)
    {
        _context = context;
        _executionResult = executionResult;
    }

    public async Task<IResult<Unit>> Handle(UserMessagesDeleteAllCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var messages = _context.Messages.Where(m => m.UserId == request.UserId).ToList();

        _context.Messages.RemoveRange(messages);
        _context.SaveChanges();

        return await _executionResult.Successful(Unit.Value);
    }
}
