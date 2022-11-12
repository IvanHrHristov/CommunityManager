using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    public class ChatroomServices : IChatroomServices
    {
        private readonly IRepository repository;
        private readonly ApplicationDbContext context;

        public ChatroomServices(
            IRepository repository,
            ApplicationDbContext context)
        {
            this.context = context;
            this.repository = repository;
        }
        public async Task<ChatroomViewModel> GetChatroomByIdAsync(Guid id)
        {
            var entity = await context.Chatrooms
                .Include(c => c.Messages)
                .Include(c => c.ChatroomsMembers)
                .ThenInclude(chm => chm.ApplicationUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return new ChatroomViewModel();
            }

            return new ChatroomViewModel()
            {
                Id = id,
                Name = entity.Name,
                Members = entity.ChatroomsMembers.Select(chm => new UserViewModel()
                {
                    Id = chm.ApplicationUserId,
                    Name = chm.ApplicationUser.UserName
                }).ToList(),
                Messages = entity?.Messages?.Select(m => new MessageViewModel()
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    Sender = m.Sender.UserName,
                    CreatedOn = m.CreatedOn
                }).ToList()
            };
        }
    }
}
