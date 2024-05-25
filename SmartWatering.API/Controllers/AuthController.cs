using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartWatering.Core.UserInfo.Auth.AccessToken;
using SmartWatering.Core.UserInfo.Auth.RefreshToken;

namespace SmartWatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthCommand query)
    {
        var data = await _mediator.Send(query);
        return data.IsSuccess
        ? Ok(data.Data)
        : StatusCode(500, data.ErrorMessage);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand query)
    {
        var data = await _mediator.Send(query);
        return data.IsSuccess
        ? Ok(data.Data)
        : StatusCode(500, data.ErrorMessage);
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        return Ok();
    }
}
