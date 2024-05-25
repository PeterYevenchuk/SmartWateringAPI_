using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Hubs;
using SmartWatering.Core.Models.SoilMoistureSensorData;
using SmartWatering.Core.WeatherSettings.Messages;
using SmartWatering.DAL.DbContext;
using SmartWatering.DAL.Models;

namespace SmartWatering.Core.Services.SoilMoistureInformationServices;

public class SoilMoistureInformationService : ISoilMoistureInformationService
{
    private readonly SwDbContext _context;
    private readonly IExecutionResult<Unit> _executionResult;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IMapper _mapper;

    public SoilMoistureInformationService(SwDbContext context, IExecutionResult<Unit> executionResult, IHubContext<MessageHub> hubContext, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _hubContext = hubContext;
        _executionResult = executionResult;
    }

    public async Task<IResult<Unit>> SaveSoilMoistureInformation(SoilMoistureInformation data)
    {
        var sendMessage = new List<ResultMessage>();

        if (data == null)
        {
            return await _executionResult.Fail("Data null!");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == data.UserId);

        if (user == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(user)).ToString());
        }

        var usersId = _context.Waterings.Where(w => w.SprinklerNameId == data.NameId).ToList();

        if (usersId == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(usersId)).ToString());
        }

        TimeSpan wastedTimeSpan = data.EndDate - data.StartDate;

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var userId in usersId)
                {
                    var result = new SensorInformation
                    {
                        StartDate = data.StartDate.ToString("dd.MM.yyyy HH:mm:ss"),
                        EndDate = data.EndDate.ToString("dd.MM.yyyy HH:mm:ss"),
                        StartLevel = data.StartLevel,
                        EndLevel = data.EndLevel,
                        WastedTime = TimeOnly.FromTimeSpan(wastedTimeSpan),
                        UserId = userId.UserId,
                        User = userId.User
                    };

                    _context.SensorInformations.Add(result);

                    var message = new ResultMessage
                    {
                        UserId = userId.UserId,
                        DayTime = InformationMessages.EndWatering,
                        DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                        Message = "Start: " + data.StartDate.ToString("dd.MM.yyyy HH:mm:ss") + ". "
                            + "End: " + data.EndDate.ToString("dd.MM.yyyy HH:mm:ss") + ". "
                            + " Wasted time: " + TimeOnly.FromTimeSpan(wastedTimeSpan).ToString("HH:mm:ss") + ". "
                            + "Before level: " + data.StartLevel + ". "
                            + "After level: " + data.EndLevel + "."
                    };

                    var resultMes = _mapper.Map<MessageModel>(message);
                    _context.Messages.Add(resultMes);
                    await _context.SaveChangesAsync();
                    sendMessage.Add(message);
                }

                await transaction.CommitAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", sendMessage);

                return await _executionResult.Successful(Unit.Value);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await _executionResult.Fail($"Failed to create user. Error: {ex.Message}");
            }
        }
    }
}
