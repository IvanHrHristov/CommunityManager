using CommunityManager.Core.Contracts;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IRepository repository;
        private readonly IChatroomServices chatroomService;

        public ChatHub(
            IRepository repository,
            IChatroomServices chatroomService)
        {
            this.repository = repository;
            this.chatroomService = chatroomService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await repository.All<ApplicationUser>()
                .Include(m => m.ChatroomsMembers)
                .ThenInclude(cm => cm.Chatroom)
                .SingleOrDefaultAsync(m => m.UserName == Context.User.Identity.Name);

            foreach (var chm in user.ChatroomsMembers)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chm.Chatroom.Id.ToString());
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessageToGroup(Guid receivingGroup, string user, string message, string userId)
        {
            await chatroomService.CreateMessageAsync(receivingGroup, userId, message);

            await Clients.Group(receivingGroup.ToString()).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(Guid group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group.ToString());
        }

        public async Task LeaveGroup(Guid group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.ToString());
        }
    }
}
