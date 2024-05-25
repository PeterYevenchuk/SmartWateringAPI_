using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.Auth.AccessToken;

public class AuthCommand : IRequest<IResult<Tokens>>
{
    public string Email { get; set; }

    public string Password { get; set; }
}
