using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartWatering.Core.Hubs;
using SmartWatering.Core.Models.SoilMoistureSensorData;
using SmartWatering.Core.Services.SoilMoistureInformationServices;
using SmartWatering.Core.Services.SoilMoistureSensorServices;
using SmartWatering.Core.WeatherSettings.Messages;
using SmartWatering.Core.WeatherSettings;
using SmartWatering.Core.WateringAutoMode;
using SmartWatering.DAL.DbContext;
using SmartWatering.Core.UserInfo.DeleteSprinkler;
using SmartWatering.Core.UserInfo.SetSprinkler;
using SmartWatering.DAL.Models.TestSprinkler;
using Microsoft.AspNetCore.Authorization;
using SmartWatering.DAL.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace SmartWatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmartWateringController : ControllerBase
{
    private readonly ISoilMoistureSensorService _soilMoistureSensorService;
    private readonly ISoilMoistureInformationService _soilMoistureInformationService;
    private readonly IMediator _mediator;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly SwDbContext _context;
    private readonly IMapper _mapper;

    public SmartWateringController(IMediator mediator, ISoilMoistureSensorService soilMoistureSensorService, 
        ISoilMoistureInformationService soilMoistureInformationService, IHubContext<MessageHub> hubContext, 
        SwDbContext context, IMapper mapper)
    {
        _mediator = mediator;
        _soilMoistureSensorService = soilMoistureSensorService;
        _soilMoistureInformationService = soilMoistureInformationService;
        _hubContext = hubContext;
        _context = context;
        _mapper = mapper;
    }

    [HttpPost("set-status/{on}/{id}")]
    public async Task<IActionResult> SetStatusTurnOnOf(bool on, int id)
    {
        var data = await _soilMoistureSensorService.SetTurnOnStatus(on, id);
        return data.IsSuccess
        ? Ok()
        : StatusCode(500, data.ErrorMessage);
    }

    [HttpPost("set-auto-mode/{on}/{id}")]
    public async Task<IActionResult> SetAutoMode(bool on, int id)
    {
        var setData = new WateringAutoModeCommand { Id = id, TurnOn = on };
        var data = await _mediator.Send(setData);
        return data.IsSuccess
        ? Ok()
        : StatusCode(500, data.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpGet("available-sprinklers")]
    public async Task<IActionResult> AvailableSprinklers()
    {
        List<string> availableSprinklers = [Sprinkler.NameId];
        return Ok(availableSprinklers);
    }

    [Authorize(Roles = "User")]
    [HttpPost("set-sprinkler")]
    public async Task<IActionResult> SetSprinkler(SetSprinklerCommand request)
    {
        var result = await _mediator.Send(request);
        return result.IsSuccess
        ? Ok()
        : StatusCode(500, result.ErrorMessage);
    }

    [Authorize(Roles = "User")]
    [HttpDelete("delete-sprinkler/{id}")]
    public async Task<IActionResult> DeleteSprinkler(int id)
    {
        var data = new DeleteSprinklerCommand { Id = id };
        var result = await _mediator.Send(data);
        return result.IsSuccess
        ? Ok()
        : StatusCode(500, result.ErrorMessage);
    }

    // for simulator
    [HttpPost("sensor-information")]
    public async Task<IActionResult> SensorInfo(SoilMoistureInformation data)
    {
        var result = await _soilMoistureInformationService.SaveSoilMoistureInformation(data);
        return result.IsSuccess
       ? Ok()
       : BadRequest(result.ErrorMessage);
    }

    [HttpPost("low-level/{level}/{nameId}")]
    public async Task<IActionResult> LowLevel(double level, string nameId)
    {
        var lowLevelMessage = new List<ResultMessage>();
        if (level <= 25)
        {
            var usersId = _context.Waterings.Include(w => w.User).Where(w => w.SprinklerNameId == nameId).ToList();
            if (usersId == null)
            {
                return BadRequest();
            }

            var userAutoMode = usersId.Find(u => u.User.AutoMode == true);
            if (userAutoMode != null)
            {
                await _soilMoistureSensorService.SetTurnOnStatus(true, userAutoMode.UserId);
            }

            foreach (var userId in usersId)
            {
                var message = new ResultMessage
                {
                    UserId = userId.UserId,
                    DayTime = DayTimeZone.Warning,
                    DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Message = string.Format(WarningMessages.LowSoilMoisture, level.ToString())
                };

                lowLevelMessage.Add(message);
                var result = _mapper.Map<MessageModel>(message);
                await _context.Messages.AddAsync(result);
            }
            _context.SaveChanges();
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", lowLevelMessage);
        }

        return Ok();
    }
}
