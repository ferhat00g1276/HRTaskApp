using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata.Ecma335;

namespace API
{
    public class ChatHub:Hub
    {
        public override async Task OnConnectedAsync()
        {
                await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} A new user has connected.");
        }

    }
}
