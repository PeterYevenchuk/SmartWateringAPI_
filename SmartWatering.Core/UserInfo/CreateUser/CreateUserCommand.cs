using MediatR;
using SmartWatering.Core.ExecutionResults;

namespace SmartWatering.Core.UserInfo.CreateUser;

public class CreateUserCommand : IRequest<IResult<Unit>>
{
    public string Name { get; set; }

    public string SurName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string CityName { get; set; }
}
