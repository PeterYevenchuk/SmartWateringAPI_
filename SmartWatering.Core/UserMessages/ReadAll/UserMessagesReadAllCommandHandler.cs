using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserMessages.ReadAll;

public class UserMessagesReadAllCommandHandler : IRequestHandler<UserMessagesReadAllCommand, IResult<Unit>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<Unit> _executionResult;

    public UserMessagesReadAllCommandHandler(SwDbContext context, IExecutionResult<Unit> executionResult)
    {
        _context = context;
        _executionResult = executionResult;
    }

    public async Task<IResult<Unit>> Handle(UserMessagesReadAllCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var messages = _context.Messages.Where(m => m.UserId == request.UserId).ToList();

        if (messages == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(messages)).ToString());
        }

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        _context.SaveChanges();

        return await _executionResult.Successful(Unit.Value);
    }
}
