using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.GetUserInformation;

public class GetUserInformationQuery : IRequest<IResult<UserDTO>>
{
    public int UserId { get; set; }
}
