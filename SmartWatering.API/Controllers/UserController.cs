using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWatering.Core.CityInfo.ChangeCityInfo;
using SmartWatering.Core.SensorInfo.GetSensorInfo;
using SmartWatering.Core.UserInfo.ChangeUserInfo;
using SmartWatering.Core.UserInfo.CreateUser;
using SmartWatering.Core.UserInfo.DeleteSprinkler;
using SmartWatering.Core.UserInfo.GetAvailableSprinklers;
using SmartWatering.Core.UserInfo.GetUserInformation;
using SmartWatering.Core.UserInfo.SetSprinkler;
using SmartWatering.DAL.Models;
using SmartWatering.DAL.Models.TestSprinkler;

namespace SmartWatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create-user")]
    public async Task<ActionResult> CreateUser(CreateUserCommand request)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess
        ? Ok(result.IsSuccess)
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpPatch("change-user-info")]
    public async Task<ActionResult> ChangeUserInfo(ChangeUserInfoCommand request)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess
        ? Ok(result.IsSuccess)
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpPatch("change-user-city")]
    public async Task<ActionResult> ChangeUserCity(ChangeCityInfoCommand request)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess
        ? Ok(result.IsSuccess)
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpGet("user-information/{id}")]
    public async Task<ActionResult> GetUserUnformation(int id)
    {
        var data = new GetUserInformationQuery { UserId = id };
        var result = await _mediator.Send(data);

        return result.IsSuccess
        ? Ok(result.Data)
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpGet("user-sensor-information/{userId}")]
    public async Task<IActionResult> GetSensorInformation(int userId)
    {
        var result = await _mediator.Send(new SensorInfoQuery { UserId = userId });
        return result.IsSuccess
        ? Ok(result.Data)
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpGet("user-available-sprinklers/{userId}")]
    public async Task<IActionResult> GetUserAvailableSprinklers(int userId)
    {
        var result = await _mediator.Send(new GetAvailableSprinklersQuery { UserId = userId });
        return result.IsSuccess
        ? Ok(result.Data)
        : StatusCode(500, result.ErrorMessage);
    }
}
