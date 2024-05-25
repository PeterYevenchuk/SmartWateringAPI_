using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.ChangeUserInfo;

public class ChangeUserInfoCommand : IRequest<IResult<Unit>>
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? SurName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? OldPassword { get; set; }
}
