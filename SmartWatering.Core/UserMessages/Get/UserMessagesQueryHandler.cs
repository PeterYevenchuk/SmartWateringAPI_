using AutoMapper;
using MediatR;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserMessages.Get;

public class UserMessagesQueryHandler : IRequestHandler<UserMessagesQuery, IResult<MessagesViewModel>>
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<MessagesViewModel> _executionResult;
    public readonly IMapper _mapper;

    public UserMessagesQueryHandler(SwDbContext context, IExecutionResult<MessagesViewModel> executionResult, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _executionResult = executionResult;
    }

    public async Task<IResult<MessagesViewModel>> Handle(UserMessagesQuery request, CancellationToken cancellationToken)
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

        var unreadCount = messages.Count(m => !m.IsRead);
        var result = new MessagesViewModel
        {
            Messages = messages,
            CountUnRead = unreadCount
        };

        return await _executionResult.Successful(result);
    }
}
