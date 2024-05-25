using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.Auth.RefreshToken;

public class RefreshTokenCommand : IRequest<IResult<Tokens>>
{
    public int UserId { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}
