using CommunityManager.Infrastructure.Data.Models;
using Microsoft.AspNetCore.SignalR;

namespace CommunityManager.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            await Clients.All.SendAsync("receiveMessage", message);
        }
    }
}
