using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserMessages.UpdateOne;

public class UserMessagesReadOneCommandHandler : IRequestHandler<UserMessagesReadOneCommand, IResult<Unit>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<Unit> _executionResult;

    public UserMessagesReadOneCommandHandler(SwDbContext context, IExecutionResult<Unit> executionResult)
    {
        _context = context;
        _executionResult = executionResult;
    }

    public async Task<IResult<Unit>> Handle(UserMessagesReadOneCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var messages = await _context.Messages.FirstOrDefaultAsync(m => m.Id == request.MessageId);

        if (messages == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(messages)).ToString());
        }

        messages.IsRead = true;
        _context.SaveChanges();

        return await _executionResult.Successful(Unit.Value);
    }
}
