using Microsoft.AspNetCore.SignalR;

namespace SmartWatering.Core.Hubs;

public class MessageHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        //await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined.");
    }
}
