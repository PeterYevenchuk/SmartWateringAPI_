using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWatering.Core.UserMessages.Delete;
using SmartWatering.Core.UserMessages.DeleteOne;
using SmartWatering.Core.UserMessages.Get;
using SmartWatering.Core.UserMessages.ReadAll;
using SmartWatering.Core.UserMessages.UpdateOne;

namespace SmartWatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "User")]
    [HttpGet("user-messages/{id}")]
    public async Task<ActionResult> GetUserMessages(int id)
    {
        var data = new UserMessagesQuery { UserId = id };
        var result = await _mediator.Send(data);

        return result.IsSuccess
        ? Ok(result.Data)
        : StatusCode(500, result.ErrorMessage);
    }

    [HttpPatch("read-message/{id}")]
    public async Task<ActionResult> UpdateUserMessage(int id)
    {
        var data = new UserMessagesReadOneCommand { MessageId = id };
        var result = await _mediator.Send(data);

        return result.IsSuccess
        ? Ok()
        : StatusCode(500, result.ErrorMessage);
    }

    [HttpPatch("read-all-messages/{id}")]
    public async Task<ActionResult> UpdateAllUserMessages(int id)
    {
        var data = new UserMessagesReadAllCommand { UserId = id };
        var result = await _mediator.Send(data);

        return result.IsSuccess
        ? Ok()
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpDelete("delete-all-message/{userId}")]
    public async Task<ActionResult> DeleteAllUserMessages(int userId)
    {
        var data = new UserMessagesDeleteAllCommand { UserId = userId };
        var result = await _mediator.Send(data);

        return result.IsSuccess
        ? Ok()
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpDelete("delete-message/{messageId}")]
    public async Task<ActionResult> DeleteUserMessages(int messageId)
    {
        var data = new UserMessageDeleteOneCommand { MessageId = messageId };
        var result = await _mediator.Send(data);

        return result.IsSuccess
        ? Ok()
        : StatusCode(500, result.ErrorMessage);
    }
}
